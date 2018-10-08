using System;
using System.Numerics;

namespace Xamla.Types.Simd
{
    public static class ArithmeticOperationsV3f
    {
        public static Vector3 Clamp(this Range<float> range, Vector3 v)
        {
            return Vector3.Min(Vector3.Max(v, new Vector3(range.Low)), new Vector3(range.High));
        }

        public static Vector3 Clamp(this Range<Vector3> range, Vector3 v)
        {
            return Vector3.Min(Vector3.Max(v, range.Low), range.High);
        }

        public static Vector3 Saturate(this Vector3 v)
        {
            return Vector3.Min(Vector3.Max(v, Vector3.Zero), Vector3.One);
        }

        public static I<Vector3> Difference(this I<Vector3> image1, I<Vector3> image2)
        {
            Vector3[] s1 = image1.Data.Buffer, s2 = image2.Data.Buffer;

            if (s1.Length != s2.Length)
                throw new ArgumentException("Image1 and image2 have to be of same format and dimensions.");

            I<Vector3> result = image1.CloneEmpty();
            var ranges = image1.Format.ChannelRanges;
            var range = new Range<Vector3>(new Vector3((float)ranges[0].Low, (float)ranges[1].Low, (float)ranges[2].Low), new Vector3((float)ranges[0].High, (float)ranges[1].High, (float)ranges[2].High));

            var r = result.Data.Buffer;
            for (int i = 0; i < s1.Length; ++i)
                r[i] = range.Clamp(s1[i] - s2[i]);

            return result;
        }

        public static I<Vector3> Add(this I<Vector3> image1, I<Vector3> image2)
        {
            Vector3[] s1 = image1.Data.Buffer, s2 = image2.Data.Buffer;

            if (s1.Length != s2.Length)
                throw new ArgumentException("Image1 and image2 have to be of same format and dimensions.");

            I<Vector3> result = image1.CloneEmpty();
            var ranges = image1.Format.ChannelRanges;
            var range = new Range<Vector3>(new Vector3((float)ranges[0].Low, (float)ranges[1].Low, (float)ranges[2].Low), new Vector3((float)ranges[0].High, (float)ranges[1].High, (float)ranges[2].High));

            var r = result.Data.Buffer;
            for (int i = 0; i < s1.Length; ++i)
                r[i] = range.Clamp(s1[i] + s2[i]);

            return result;
        }

        public static I<Vector3> Multiply(this I<Vector3> image1, I<Vector3> image2)
        {
            Vector3[] s1 = image1.Data.Buffer, s2 = image2.Data.Buffer;

            if (s1.Length != s2.Length)
                throw new ArgumentException("Image1 and image2 have to be of same format and dimensions.");

            I<Vector3> result = image1.CloneEmpty();
            var ranges = image1.Format.ChannelRanges;
            var range = new Range<Vector3>(new Vector3((float)ranges[0].Low, (float)ranges[1].Low, (float)ranges[2].Low), new Vector3((float)ranges[0].High, (float)ranges[1].High, (float)ranges[2].High));

            var r = result.Data.Buffer;
            for (int i = 0; i < s1.Length; ++i)
                r[i] = range.Clamp(s1[i] * s2[i]);

            return result;
        }

        public static I<Vector3> Divide(this I<Vector3> image1, I<Vector3> image2)
        {
            Vector3[] s1 = image1.Data.Buffer, s2 = image2.Data.Buffer;

            if (s1.Length != s2.Length)
                throw new ArgumentException("Image1 and image2 have to be of same format and dimensions.");

            I<Vector3> result = image1.CloneEmpty();
            var ranges = image1.Format.ChannelRanges;
            var range = new Range<Vector3>(new Vector3((float)ranges[0].Low, (float)ranges[1].Low, (float)ranges[2].Low), new Vector3((float)ranges[0].High, (float)ranges[1].High, (float)ranges[2].High));

            var r = result.Data.Buffer;
            for (int i = 0; i < s1.Length; ++i)
                r[i] = range.Clamp(s1[i] / s2[i]);

            return result;
        }
    }
}
