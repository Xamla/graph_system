using System.Numerics;

namespace Xamla.Types.Simd
{
    public static class F32Conversion
    {
        public static I<Vector3> ToVector3(this I<Rgb24> input)
        {
            var ranges = new Range<double>[input.Format.ChannelRanges.Length];
            for (int i = 0; i < ranges.Length; ++i)
                ranges[i] = Range.Unit;

            var format = new PixelFormat(PixelType.F32C3, input.Format.PixelChannels, typeof(Vector3), ranges, input.Format.ColorSpace);
            var output = new I<Vector3>(format, input.Height, input.Width);

            var src = input.Data.Buffer;
            var dst = output.Data.Buffer;

            for (int i = 0; i < src.Length; i += 1)
            {
                var p = src[i];
                dst[i] = new Vector3(p.R / 255.0f, p.G / 255.0f, p.B / 255.0f).Saturate();
            }

            return output;
        }

        public static I<Vector3> ToF32(this I<byte> input)
        {
            var ranges = new Range<double>[input.Format.ChannelRanges.Length];
            for (int i = 0; i < ranges.Length; ++i)
                ranges[i] = Range.Unit;

            var format = new PixelFormat(PixelType.F32C3, input.Format.PixelChannels, typeof(Vector3), ranges, input.Format.ColorSpace);
            var output = new I<Vector3>(format, input.Height, input.Width);

            var src = input.Data.Buffer;
            var dst = output.Data.Buffer;

            for (int i = 0, j = 0; i < src.Length; i += 3, j += 1)
            {
                dst[j] = new Vector3(src[i + 0] / 255.0f, src[i + 1] / 255.0f, src[i + 2] / 255.0f).Saturate();
            }

            return output;
        }

        public static I<Vector3> ToVector3(this I<float> input)
        {
            var format = new PixelFormat(input.Format.PixelType, input.Format.PixelChannels, typeof(Vector3), (Range<double>[])input.Format.ChannelRanges.Clone(), input.Format.ColorSpace);
            var output = new I<Vector3>(format, input.Height, input.Width);

            for (int y = 0; y < input.Height; ++y)
            {
                for (int x = 0; x < input.Width; ++x)
                {
                    output[y, x] = new Vector3(input[y, x, 0], input[y, x, 1], input[y, x, 2]);
                }
            }

            return output;
        }

        public static I<Vector4> ToVector4f(this I<float> input)
        {
            var format = new PixelFormat(input.Format.PixelType, input.Format.PixelChannels, typeof(Vector4), (Range<double>[])input.Format.ChannelRanges.Clone(), input.Format.ColorSpace);
            var output = new I<Vector4>(format, input.Height, input.Width);
            int channels = input.Channels;
            if (channels == 1)
            {
                for (int y = 0; y < input.Height; ++y)
                {
                    for (int x = 0; x < input.Width; ++x)
                    {
                        output[y, x] = new Vector4(input[y, x]);
                    }
                }
            }
            else if (channels == 4)
            {
                for (int y = 0; y < input.Height; ++y)
                {
                    for (int x = 0; x < input.Width; ++x)
                    {
                        output[y, x] = new Vector4(input[y, x, 0], input[y, x, 1], input[y, x, 2], input[y, x, 3]);
                    }
                }
            }
            else
            {
                float a, b, c, d;
                for (int y = 0; y < input.Height; ++y)
                {
                    for (int x = 0; x < input.Width; ++x)
                    {
                        a = (channels >= 1) ? input[y, x, 0] : 0;
                        b = (channels >= 2) ? input[y, x, 1] : 0;
                        c = (channels >= 3) ? input[y, x, 2] : 0;
                        d = (channels >= 4) ? input[y, x, 3] : 0;
                        output[y, x] = new Vector4(a, b, c, d);
                    }
                }
            }

            return output;
        }

        public static I<float> ToF32(this I<Vector3> input)
        {
            var format = new PixelFormat(input.Format.PixelType, input.Format.PixelChannels, typeof(float), (Range<double>[])input.Format.ChannelRanges.Clone(), input.Format.ColorSpace);
            var output = new I<float>(format, input.Height, input.Width, 3);

            var s = input.Data.Buffer;
            var d = output.Data.Buffer;
            for (int i = 0, j = 0; i < s.Length; i += 1, j += 3)
                s[i].CopyTo(d, j);

            return output;
        }

        public static I<float> ToF32(this I<Vector4> input)
        {
            var format = new PixelFormat(input.Format.PixelType, input.Format.PixelChannels, typeof(float), (Range<double>[])input.Format.ChannelRanges.Clone(), input.Format.ColorSpace);
            var output = new I<float>(format, input.Height, input.Width, 4);

            var s = input.Data.Buffer;
            var d = output.Data.Buffer;
            for (int i = 0, j = 0; i < s.Length; i += 1, j += 4)
                s[i].CopyTo(d, j);

            return output;
        }
    }
}
