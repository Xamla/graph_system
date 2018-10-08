using System;
using System.Linq;
using System.Threading;

namespace Xamla.Types
{
    public enum ResizeFilterType
    {
        Hermite,
        NearestNeighbor,
        Triangle,
        Bell,
        CubicBSpline,
        Lanczos3,
        Mitchell,
        Cosine,
        CatmullRom,
        Quadratic,
        QuadraticBSpline,
        CubicConvolution,
        Lanczos8
    }

    public static class ImageBufferResize
    {
        public static I<float> Resize(this I<float> source, Int2 targetSize, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            return Resize(source, targetSize.Y, targetSize.X, filterType, cancel);
        }

        public static I<float> Resize(this I<float> source, Int2 targetSize, IntRect sourceRec, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            return Resize(source, targetSize.Y, targetSize.X, sourceRec, filterType, cancel);
        }

        public static I<float> Resize(this I<float> source, int targetHeight, int targetWidth, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            var destination = new I<float>(source.Format, targetHeight, targetWidth);
            Resize(source, destination, filterType, cancel);
            return destination;
        }

        public static I<float> Resize(this I<float> source, int targetHeight, int targetWidth, IntRect sourceRect, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            var destination = new I<float>(source.Format, targetHeight, targetWidth);
            Resize(source, sourceRect, destination, filterType, cancel);
            return destination;
        }

        public static void Resize(I<float> source, I<float> destination, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            Resize(source, new IntRect(0, 0, source.Width, source.Height), destination, filterType, cancel);
        }

        public static void Resize(I<float> source, IntRect sourceRect, I<float> destination, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
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

            int sourceChannelCount = source.Format.ChannelCount;
            int destinationChannelCount = destination.Format.ChannelCount;
            int sourceStride = source.Width * source.Format.ChannelCount;
            int destinationStride = destination.Width * destination.Format.ChannelCount;

            OutputPixel[] contribX = CreateContributors(filterType, source.Width, sourceRect.Left, sourceRect.Right, destination.Width);
            OutputPixel[] contribY = CreateContributors(filterType, source.Height, sourceRect.Top, sourceRect.Bottom, destination.Height);

            float filterRadius = GetFilterRadius(filterType);
            int ringBufferLines = contribY.Max(x => x.N);
            var ringBuffer = new float[destinationStride * ringBufferLines];

            var ranges = destination.Format.ChannelRanges.Select(r => Range.Create((float)r.Low, (float)r.High)).ToArray();

            int sY = sourceRect.Top;
            int dX, j;
            int offset = 0;
            float value;
            int positionInRing = sY % ringBufferLines;

            for (int dY = 0; dY < destination.Height; ++dY)
            {
                if ((dY % 128) == 0 && cancel.IsCancellationRequested)
                    break;

                int rowIn, rowOut;
                while (sY < sourceRect.Bottom && sY <= contribY[dY].MaxPixel)
                {
                    rowIn = sY * sourceStride;
                    rowOut = positionInRing * destinationStride;

                    for (dX = 0; dX < destination.Width; ++dX)
                    {
                        var contrib = contribX[dX];
                        for (int c = 0; c < destinationChannelCount; ++c)
                        {
                            value = 0;

                            for (j = 0; j < contrib.N; ++j)
                            {
                                float weight = contrib.P[j].Weight;
                                if (weight != 0)
                                {
                                    offset = contrib.P[j].Pixel * sourceChannelCount;
                                    value = value + s[rowIn + offset + c] * weight;
                                }
                            }

                            ringBuffer[rowOut] = value * contrib.Normalize;
                            ++rowOut;
                        }
                    }

                    ++sY;
                    positionInRing = (positionInRing + 1) % ringBufferLines;
                }

                // resize lines vertically
                for (int k = 0; k < destinationStride;)
                {
                    var contrib = contribY[dY];
                    for (int c = 0; c < destinationChannelCount; ++c, ++k)
                    {
                        value = 0;

                        for (j = 0; j < contrib.N; ++j)
                        {
                            float weight = contrib.P[j].Weight;
                            if (weight != 0)
                            {
                                offset = (contrib.P[j].Pixel % ringBufferLines) * destinationStride + k;
                                value = value + ringBuffer[offset] * weight;
                            }
                        }

                        d[dY * destinationStride + k] = ranges[c].Clamp((float)(value * contrib.Normalize));
                    }
                }
            }
        }

        public static I<double> Resize(this I<double> source, Int2 targetSize, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            return Resize(source, targetSize.Y, targetSize.X, filterType, cancel);
        }

        public static I<double> Resize(this I<double> source, Int2 targetSize, IntRect sourceRec, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            return Resize(source, targetSize.Y, targetSize.X, sourceRec, filterType, cancel);
        }

        public static I<double> Resize(this I<double> source, int targetHeight, int targetWidth, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            var destination = new I<double>(source.Format, targetHeight, targetWidth);
            Resize(source, destination, filterType, cancel);
            return destination;
        }

        public static I<double> Resize(this I<double> source, int targetHeight, int targetWidth, IntRect sourceRect, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            var destination = new I<double>(source.Format, targetHeight, targetWidth);
            Resize(source, sourceRect, destination, filterType, cancel);
            return destination;
        }

        public static void Resize(I<double> source, I<double> destination, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
        {
            Resize(source, new IntRect(0, 0, source.Width, source.Height), destination, filterType, cancel);
        }

        public static void Resize(I<double> source, IntRect sourceRect, I<double> destination, ResizeFilterType filterType, CancellationToken cancel = default(CancellationToken))
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

            int sourceChannelCount = source.Format.ChannelCount;
            int destinationChannelCount = destination.Format.ChannelCount;
            int sourceStride = source.Width * source.Format.ChannelCount;
            int destinationStride = destination.Width * destination.Format.ChannelCount;

            OutputPixel[] contribX = CreateContributors(filterType, source.Width, sourceRect.Left, sourceRect.Right, destination.Width);
            OutputPixel[] contribY = CreateContributors(filterType, source.Height, sourceRect.Top, sourceRect.Bottom, destination.Height);

            double filterRadius = GetFilterRadius(filterType);
            int ringBufferLines = contribY.Max(x => x.N);
            var ringBuffer = new double[destinationStride * ringBufferLines];

            var ranges = destination.Format.ChannelRanges.Select(r => Range.Create((double)r.Low, (double)r.High)).ToArray();

            int sY = sourceRect.Top;
            int dX, j;
            int offset = 0;
            double value;
            int positionInRing = sY % ringBufferLines;

            for (int dY = 0; dY < destination.Height; ++dY)
            {
                if ((dY % 128) == 0 && cancel.IsCancellationRequested)
                    break;

                int rowIn, rowOut;
                while (sY < sourceRect.Bottom && sY <= contribY[dY].MaxPixel)
                {
                    rowIn = sY * sourceStride;
                    rowOut = positionInRing * destinationStride;

                    for (dX = 0; dX < destination.Width; ++dX)
                    {
                        var contrib = contribX[dX];
                        for (int c = 0; c < destinationChannelCount; ++c)
                        {
                            value = 0;

                            for (j = 0; j < contrib.N; ++j)
                            {
                                double weight = contrib.P[j].Weight;
                                if (weight != 0)
                                {
                                    offset = contrib.P[j].Pixel * sourceChannelCount;
                                    value = value + s[rowIn + offset + c] * weight;
                                }
                            }

                            ringBuffer[rowOut] = value * contrib.Normalize;
                            ++rowOut;
                        }
                    }

                    ++sY;
                    positionInRing = (positionInRing + 1) % ringBufferLines;
                }

                // resize lines vertically
                for (int k = 0; k < destinationStride;)
                {
                    var contrib = contribY[dY];
                    for (int c = 0; c < destinationChannelCount; ++c, ++k)
                    {
                        value = 0;

                        for (j = 0; j < contrib.N; ++j)
                        {
                            double weight = contrib.P[j].Weight;
                            if (weight != 0)
                            {
                                offset = (contrib.P[j].Pixel % ringBufferLines) * destinationStride + k;
                                value = value + ringBuffer[offset] * weight;
                            }
                        }

                        d[dY * destinationStride + k] = ranges[c].Clamp((double)(value * contrib.Normalize));
                    }
                }
            }
        }

        internal struct InputPixel
        {
            public int Pixel;
            public float Weight;
        };

        internal struct OutputPixel
        {
            public int N;
            public int MinPixel;
            public int MaxPixel;
            public InputPixel[] P;
            public float Normalize;
        };

        static double SinC(double x)
        {
            if (x != 0)
            {
                x *= Math.PI;
                return Math.Sin(x) / x;
            }

            return 1;
        }

        static OutputPixel[] CreateContributor(int inputWidth, int leftIn, int rightIn, int outputWidth, float filterRadius, Func<float, float> calcWeight)
        {
            var contributor = new OutputPixel[outputWidth];

            float scale = (float)outputWidth / (rightIn - leftIn);
            float factor = scale < 1 ? scale : 1;
            float sourceWidth = filterRadius / factor;
            int cbbStride = (int)Math.Ceiling(2 * sourceWidth) + 1;

            for (int i = 0; i < outputWidth; ++i)
            {
                var o = new OutputPixel();
                o.P = new InputPixel[cbbStride];

                if (filterRadius > 0)
                {
                    double sum = 0;
                    int n = 0;
                    float center = leftIn + (i + 0.5f) / scale;
                    int left = (int)Math.Floor(center - sourceWidth);
                    int right = left + cbbStride - 1;
                    for (int j = left; j <= right; ++j)
                    {
                        float weight = calcWeight((center - j - 0.5f) * factor);

                        o.P[n].Pixel = j;

                        // reflection
                        if (o.P[n].Pixel < 0)
                            o.P[n].Pixel = Math.Min(-o.P[n].Pixel, inputWidth);
                        if (o.P[n].Pixel >= inputWidth)
                            o.P[n].Pixel = Math.Max(2 * inputWidth - o.P[n].Pixel - 2, 0);

                        o.P[n].Weight = weight;
                        ++n;
                        sum += weight;
                        o.MinPixel = left;
                        o.MaxPixel = right;
                    }

                    o.N = n;
                    o.Normalize = (float)(1.0 / sum);
                }
                else
                {
                    // nearest neighbor case
                    o.MinPixel = leftIn + (int)((i + 0.5f) / scale);
                    o.MaxPixel = o.MinPixel;
                    o.P = new InputPixel[] { new InputPixel { Pixel = o.MinPixel, Weight = calcWeight(0) } };
                    o.N = 1;
                    o.Normalize = 1;
                }

                contributor[i] = o;
            }
            return contributor;
        }

        static OutputPixel[] Hermite(int inputWidth, int leftIn, int rightIn, int widthOut)
        {
            const float radius = 1.0f;
            return CreateContributor(inputWidth, leftIn, rightIn, widthOut, radius, x =>
            {
                if (x < 0) x = -x;
                if (x < 1)
                    return ((2 * x - 3) * x * x + 1);
                else
                    return 0;
            });
        }

        static OutputPixel[] NearestNeighbor(int inputWidth, int leftIn, int rightIn, int widthOut)
        {
            return CreateContributor(inputWidth, leftIn, rightIn, widthOut, 0, x => 1);
        }

        static OutputPixel[] Triangle(int inputWidth, int leftIn, int rightIn, int widthOut)
        {
            const float radius = 1.0f;
            return CreateContributor(inputWidth, leftIn, rightIn, widthOut, radius, x =>
            {
                if (x < 0) x = -x;
                if (x < 1)
                    return 1 - x;
                else
                    return 0;
            });
        }

        static OutputPixel[] CubicBSpline(int inputWidth, int leftIn, int rightIn, int widthOut)
        {
            const float radius = 2.0f;
            return CreateContributor(inputWidth, leftIn, rightIn, widthOut, radius, x =>
            {
                if (x < 0) x = -x;
                if (x < 1)
                {
                    var x2 = x * x;
                    return 0.5f * x2 * x - x2 + 2.0f / 3.0f;
                }
                else if (x < 2)
                    return (float)Math.Pow(2 - x, 3) / 6;
                else
                    return 0;
            });
        }

        static OutputPixel[] Lanczos3(int inputWidth, int leftIn, int rightIn, int widthOut)
        {
            const float radius = 2.0f;
            return CreateContributor(inputWidth, leftIn, rightIn, widthOut, radius, x =>
            {
                if (x < 0) x = -x;
                if (x < 3)
                    return (float)(SinC(x) * SinC(x / 3));
                else
                    return 0;
            });
        }

        static OutputPixel[] Mitchell(int inputWidth, int leftIn, int rightIn, int widthOut)
        {
            const float radius = 2.0f;
            const float C = 1.0f / 3.0f;

            return CreateContributor(inputWidth, leftIn, rightIn, widthOut, radius, x =>
            {
                if (x < 0) x = -x;
                var x2 = x * x;
                if (x < 1)
                    return ((((12.0f - 9.0f * C - 6.0f * C) * (x * x2)) + ((-18.0f + 12.0f * C + 6.0f * C) * x2) + (6.0f - 2.0f * C))) / 6.0f;
                else if (x < 2)
                    return ((((-1.0f * C - 6.0f * C) * (x * x2)) + ((6.0f * C + 30.0f * C) * x2) + ((-12.0f * C - 48.0f * C) * x) + (8.0f * C + 24.0f * C))) / 6.0f;
                else
                    return 0;
            });
        }

        static OutputPixel[] Cosine(int inputWidth, int leftIn, int rightIn, int widthOut)
        {
            const float radius = 1.0f;
            return CreateContributor(inputWidth, leftIn, rightIn, widthOut, radius, x =>
            {
                if (x >= -1.0f && x <= 1.0f)
                    return (float)((Math.Cos(x * Math.PI) + 1.0) / 2.0);
                else
                    return 0;
            });
        }

        static OutputPixel[] CatmullRom(int inputWidth, int leftIn, int rightIn, int widthOut)
        {
            const float radius = 2.0f;
            return CreateContributor(inputWidth, leftIn, rightIn, widthOut, radius, x =>
            {
                if (x < 0) x = -x;
                var x2 = x * x;
                if (x <= 1.0f)
                    return 1.5f * x2 * x - 2.5f * x2 + 1.0f;
                else if (x <= 2.0f)
                    return -0.5f * x2 * x + 2.5f * x2 - 4.0f * x + 2.0f;
                else
                    return 0;
            });
        }

        static OutputPixel[] Bell(int inputWidth, int leftIn, int rightIn, int widthOut)
        {
            const float radius = 1.5f;
            return CreateContributor(inputWidth, leftIn, rightIn, widthOut, radius, x =>
            {
                if (x < 0) x = -x;
                if (x < 0.5f)
                    return 0.75f - x * x;
                else if (x < 1.5f)
                    return (float)(0.5 * Math.Pow(x - 1.5f, 2));
                else
                    return 0;
            });
        }

        static OutputPixel[] Quadratic(int inputWidth, int leftIn, int rightIn, int widthOut)
        {
            const float radius = 1.5f;
            return CreateContributor(inputWidth, leftIn, rightIn, widthOut, radius, x =>
            {
                if (x < 0) x = -x;
                if (x <= 0.5f)
                    return -2 * x * x + 1;
                else if (x <= 1.5f)
                    return x * x - 2.5f * x + 1.5f;
                else
                    return 0;
            });
        }

        static OutputPixel[] QuadraticBSpline(int inputWidth, int leftIn, int rightIn, int widthOut)
        {
            const float radius = 1.5f;
            return CreateContributor(inputWidth, leftIn, rightIn, widthOut, radius, x =>
            {
                if (x < 0) x = -x;
                if (x <= 0.5f)
                    return -1 * x * x + 0.75f;
                else if (x <= 1.5f)
                    return 0.5f * x * x - 1.5f * x + 1.125f;
                else
                    return 0;
            });
        }

        static OutputPixel[] CubicConvolution(int inputWidth, int leftIn, int rightIn, int widthOut)
        {
            const float radius = 3.0f;
            return CreateContributor(inputWidth, leftIn, rightIn, widthOut, radius, x =>
            {
                if (x < 0) x = -x;
                var x2 = x * x;
                if (x <= 1)
                    return ((4.0f / 3.0f) * x2 * x - (7.0f / 3.0f) * x2 + 1);
                else if (x <= 2.0f)
                    return (-(7.0f / 12.0f) * x2 * x + 3 * x2 - (59.0f / 12.0f) * x + 2.5f);
                else if (x <= 3.0f)
                    return ((1.0f / 12.0f) * x2 * x - (2.0f / 3.0f) * x2 + 1.75f * x - 1.5f);
                else
                    return 0;
            });
        }

        static OutputPixel[] Lanczos8(int inputWidth, int leftIn, int rightIn, int widthOut)
        {
            const float radius = 8.0f;
            return CreateContributor(inputWidth, leftIn, rightIn, widthOut, radius, x =>
            {
                if (x < 0) x = -x;
                if (x < 8)
                    return (float)(SinC(x) * SinC(x / 8));
                else
                    return 0;
            });
        }

        internal static OutputPixel[] CreateContributors(ResizeFilterType filterType, int inputWidth, int leftIn, int rightIn, int widthOut)
        {
            switch (filterType)
            {
                case ResizeFilterType.Hermite:
                    return Hermite(inputWidth, leftIn, rightIn, widthOut);
                case ResizeFilterType.Triangle:
                    return Triangle(inputWidth, leftIn, rightIn, widthOut);
                case ResizeFilterType.CubicBSpline:
                    return CubicBSpline(inputWidth, leftIn, rightIn, widthOut);
                case ResizeFilterType.Lanczos3:
                    return Lanczos3(inputWidth, leftIn, rightIn, widthOut);
                case ResizeFilterType.Mitchell:
                    return Mitchell(inputWidth, leftIn, rightIn, widthOut);
                case ResizeFilterType.Cosine:
                    return Cosine(inputWidth, leftIn, rightIn, widthOut);
                case ResizeFilterType.CatmullRom:
                    return CatmullRom(inputWidth, leftIn, rightIn, widthOut);
                case ResizeFilterType.QuadraticBSpline:
                    return QuadraticBSpline(inputWidth, leftIn, rightIn, widthOut);
                case ResizeFilterType.Bell:
                    return Bell(inputWidth, leftIn, rightIn, widthOut);
                case ResizeFilterType.Quadratic:
                    return Quadratic(inputWidth, leftIn, rightIn, widthOut);
                case ResizeFilterType.CubicConvolution:
                    return CubicConvolution(inputWidth, leftIn, rightIn, widthOut);
                case ResizeFilterType.Lanczos8:
                    return Lanczos8(inputWidth, leftIn, rightIn, widthOut);
                case ResizeFilterType.NearestNeighbor:
                default:
                    return NearestNeighbor(inputWidth, leftIn, rightIn, widthOut);
            }
        }

        internal static float GetFilterRadius(ResizeFilterType filterType)
        {
            switch (filterType)
            {
                case ResizeFilterType.Hermite:
                    return 1.0f;
                case ResizeFilterType.NearestNeighbor:
                    return 0.5f;
                case ResizeFilterType.Triangle:
                    return 1.0f;
                case ResizeFilterType.Bell:
                    return 1.5f;
                case ResizeFilterType.CubicBSpline:
                    return 2.0f;
                case ResizeFilterType.Lanczos3:
                    return 3.0f;
                case ResizeFilterType.Mitchell:
                    return 2.0f;
                case ResizeFilterType.Cosine:
                    return 1.0f;
                case ResizeFilterType.CatmullRom:
                    return 2.0f;
                case ResizeFilterType.Quadratic:
                    return 1.5f;
                case ResizeFilterType.QuadraticBSpline:
                    return 1.5f;
                case ResizeFilterType.CubicConvolution:
                    return 3.0f;
                case ResizeFilterType.Lanczos8:
                    return 8.0f;
            }

            throw new NotSupportedException("Invalid resize filter type");
        }
    }
}
