using System;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace Xamla.Types.Simd
{
    public static class ResizeV4
    {
        public static I<Vector4> Resize(this I<Vector4> source, Int2 targetSize, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            return Resize(source, targetSize.Y, targetSize.X, filterType, cancel);
        }

        public static I<Vector4> Resize(this I<Vector4> source, Int2 targetSize, IntRect sourceRec, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            return Resize(source, targetSize.Y, targetSize.X, sourceRec, filterType, cancel);
        }

        public static I<Vector4> Resize(this I<Vector4> source, int targetHeight, int targetWidth, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            var destination = new I<Vector4>(source.Format, targetHeight, targetWidth);
            Resize(source, destination, filterType, cancel);
            return destination;
        }

        public static I<Vector4> Resize(this I<Vector4> source, int targetHeight, int targetWidth, IntRect sourceRect, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            var destination = new I<Vector4>(source.Format, targetHeight, targetWidth);
            Resize(source, sourceRect, destination, filterType, cancel);
            return destination;
        }

        public static void Resize(I<Vector4> source, I<Vector4> destination, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            Resize(source, new IntRect(0, 0, source.Width, source.Height), destination, filterType, cancel);
        }

        public static void Resize(I<Vector4> source, IntRect sourceRect, I<Vector4> destination, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            if (source == null)
                throw new ArgumentNullException("buffer");

            if (destination == null)
                throw new ArgumentNullException("destination");

            sourceRect = sourceRect.Clip(new IntRect(0, 0, source.Width, source.Height));
            if (sourceRect.Width <= 0 || sourceRect.Height <= 0 || source.Channels <= 0)
                throw new ArgumentNullException("Source image must not be empty");

            if (destination.Width <= 0 || destination.Height <= 0 || destination.Channels <= 0)
                return;

            var s = source.Data.Buffer;
            var d = destination.Data.Buffer;

            var contribX = ImageBufferResize.CreateContributors(filterType, source.Width, sourceRect.Left, sourceRect.Right, destination.Width);
            var contribY = ImageBufferResize.CreateContributors(filterType, source.Height, sourceRect.Top, sourceRect.Bottom, destination.Height);

            float filterRadius = ImageBufferResize.GetFilterRadius(filterType);
            int ringBufferLines = contribY.Max(x => x.N);
            var ringBuffer = new Vector4[destination.Width * ringBufferLines];

            var ranges = destination.Format.ChannelRanges;
            var low = new Vector4((float)ranges[0].Low, (float)ranges[1].Low, (float)ranges[2].Low, (float)ranges[3].Low);
            var high = new Vector4((float)ranges[0].High, (float)ranges[1].High, (float)ranges[2].High, (float)ranges[3].High);

            int sY = sourceRect.Top;
            int dX, j;
            int offset = 0;
            int positionInRing = sY % ringBufferLines;

            for (int dY = 0; dY < destination.Height; ++dY)
            {
                if ((dY % 128) == 0 && cancel.IsCancellationRequested)
                    break;

                int rowIn, rowOut;
                while (sY < sourceRect.Bottom && sY <= contribY[dY].MaxPixel)
                {
                    rowIn = sY * source.Width;
                    rowOut = positionInRing * destination.Width;

                    for (dX = 0; dX < destination.Width; ++dX)
                    {
                        var value = Vector4.Zero;
                        for (j = 0; j < contribX[dX].N; ++j)
                        {
                            float weight = contribX[dX].P[j].Weight;
                            if (weight != 0)
                            {
                                offset = contribX[dX].P[j].Pixel;
                                value = value + s[rowIn + offset] * weight;
                            }
                        }

                        ringBuffer[rowOut] = value * contribX[dX].Normalize;
                        ++rowOut;
                    }

                    ++sY;
                    positionInRing = (positionInRing + 1) % ringBufferLines;
                }

                // resize lines vertically
                for (int k = 0; k < destination.Width; ++k)
                {
                    var value = Vector4.Zero;

                    for (j = 0; j < contribY[dY].N; ++j)
                    {
                        float weight = contribY[dY].P[j].Weight;
                        if (weight != 0)
                        {
                            offset = contribY[dY].P[j].Pixel % ringBufferLines;
                            offset = offset * destination.Width + k;
                            value = value + ringBuffer[offset] * weight;
                        }
                    }

                    value = Vector4.Max(Vector4.Min(value * contribY[dY].Normalize, high), low);
                    d[dY * destination.Width + k] = value;
                }
            }
        }
    }
}
