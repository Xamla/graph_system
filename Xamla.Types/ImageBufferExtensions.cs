using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types
{
    public static class ImageBufferExtensions
    {
        public static I<float> ToF32(this IImageBuffer source)
        {
            if (source == null)
                throw new ArgumentNullException("buffer");

            var t0 = source as I<float>;
            if (t0 != null)
                return t0;

            var t1 = source as I<byte>;
            if (t1 != null)
                return t1.ToF32();

            var t2 = source as I<Rgb24>;
            if (t2 != null)
                return t2.ToRgbF32();

            throw new NotSupportedException(string.Format("Conversion routine from buffer image format to 32 bit per channel is missing (buffer pixel format: was {0}).", source.Format));
        }

        public static I<byte> ToU8(this IImageBuffer source)
        {
            if (source == null)
                throw new ArgumentNullException("buffer");

            var t0 = source as I<byte>;
            if (t0 != null)
                return t0;

            var t1 = source as I<float>;
            if (t1 != null)
            {
                return t1.ToU8();
            }
            else
            {
                var t2 = source as I<Rgb24>;
                if (t2 != null)
                    return I.FromBytes<byte>(t2.ToByteArray(), t2.Height, t2.Width, PixelFormat.RgbU8);
            }

            throw new NotSupportedException(string.Format("Conversion routine from buffer image format to 8 bit per channel is missing (buffer pixel format: was {0}).", source.Format));
        }
    }
}
