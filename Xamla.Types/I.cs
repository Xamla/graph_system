using System;
using System.Runtime.InteropServices;
using System.Reflection;
using Xamla.Utilities;

namespace Xamla.Types
{
    [Serializable]
    public sealed class I<T>
        : IImageBuffer
    {
        internal PixelFormat format;
        A<T> data;

        public I(PixelFormat format, int height, int width, int channels)
            : this(format, (channels > 1) ? new A<T>(height, width, channels) : new A<T>(height, width))
        {
        }

        public I(PixelFormat format, int height, int width)
            : this(format, (format.ElementType.GetTypeInfo().IsPrimitive && format.ChannelCount > 1) ? new A<T>(height, width, format.ChannelCount) : new A<T>(height, width))
        {
        }

        public I(PixelFormat format, A<T> data)
        {
            if (format.ElementType != typeof(T))
                throw new ArgumentException("PixelFormat is not compatible to image pixel data type.");

            this.format = format;
            this.data = data;
        }

        public I(PixelFormat format, A data)
            : this(format, (A<T>)data)
        {
        }

        public PixelFormat Format => format;
        public int Height => data.Dimension[0];
        public int Width => data.Dimension[1];

        public int Channels
        {
            get
            {
                if (data.Rank < 3)
                    return 1;

                return data.Dimension[2];
            }
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is I<T> i))
                return false;

            return this.data.Equals(i.data) && this.format.Equals(i.format);
        }

        public override int GetHashCode()
        {
            return this.data.GetHashCode() + this.format.GetHashCode();
        }

        public PinnedGCHandle Pin()
        {
            return data.Pin();
        }

        public long SizeInBytes => data.SizeInBytes;
        A IImageBuffer.Data => data;
        public A<T> Data => data;

        public T this[int y, int x]
        {
            get { return data[y, x]; }
            set { data[y, x] = value; }
        }

        public T this[int y, int x, int c]
        {
            get { return data[y, x, c]; }
            set { data[y, x, c] = value; }
        }

        public byte[] ToByteArray()
        {
            var a = new byte[this.SizeInBytes];
            using (var p = data.Pin())
            {
                Marshal.Copy(p.Pointer, a, 0, a.Length);
            }
            return a;
        }

        public int PixelCount
        {
            get { return this.Height * this.Width; }
        }

        public I<T> Clone()
        {
            return new I<T>(this.Format, this.data.Clone());
        }

        public I<T> CloneEmpty()
        {
            return new I<T>(this.Format, this.Height, this.Width, this.Channels);
        }

        IImageBuffer IImageBuffer.Clone()
        {
            return Clone();
        }

        I<T> PadZero(int top, int left, int bottom, int right)
        {
            int h = this.Height + top + bottom;
            int w = this.Width + left + right;

            int elementSize = Marshal.SizeOf<T>();
            int channels = (data.Rank > 2 ? data.Dimension[2] : 1);
            int pixelSize = elementSize * channels;

            I<T> r = new I<T>(this.Format, h, w, channels);
            T[] src = this.Data.Buffer;
            T[] dst = r.Data.Buffer;

            if (typeof(T).GetTypeInfo().IsPrimitive)
            {
                int stride = w * pixelSize;
                int d = stride * top + left * pixelSize;

                for (int y = 0; y < this.Height; y += 1, d += stride)
                {
                    Buffer.BlockCopy(src, y * this.Width * pixelSize, dst, d, this.Width * pixelSize);
                }
            }
            else
            {
                int stride = w;
                int d = stride * top + left;

                for (int y = 0; y < this.Height; y += 1, d += stride)
                {
                    Array.Copy(src, y * this.Width, dst, d, this.Width);
                }
            }

            return r;
        }

        public I<T> AddBorder(int top, int left, int bottom, int right, BorderMode mode = BorderMode.Zero)
        {
            var r = PadZero(top, left, bottom, right);
            switch (mode)
            {
                case BorderMode.Replicate:
                    r.FillBorderReplicate(top, left, bottom, right);
                    break;
                case BorderMode.Reflect:
                    r.FillBorderReflect(top, left, bottom, right);
                    break;
            }
            return r;
        }

        public I<T> AddBorder(int top, int left, int bottom, int right, params T[] color)
        {
            var r = PadZero(top, left, bottom, right);
            FillBorderConstant(top, left, bottom, right, color);
            return r;
        }

        private static int[] ReflectOffsets(int dy, int dh, int sy, int sh)
        {
            var offs = new int[dh];
            int y;
            for (int i = dy; i < dy + dh; ++i)
            {
                y = Math.Abs(i - sy);

                if ((y / sh) % 2 == 0)
                    y %= sh;
                else
                    y = sh - 1 - (y % sh);

                offs[i - dy] = sy + y;
            }

            return offs;
        }

        private void FillReflect(int dy, int dx, int dh, int dw, int sy, int sx, int sh, int sw)
        {
            int channels = data.Rank > 2 ? data.Dimension[2] : 1;
            int y, x;

            int[] yoffs = ReflectOffsets(dy, dh, sy, sh);
            int[] xoffs = ReflectOffsets(dx, dw, sx, sw);

            if (channels > 1)
            {
                for (int i = 0; i < dh; ++i)
                {
                    y = yoffs[i];
                    for (int j = 0; j < dw; ++j)
                    {
                        x = xoffs[j];
                        for (int c = 0; c < channels; ++c)
                            this[dy + i, dx + j, c] = this[y, x, c];
                    }
                }
            }
            else
            {
                for (int i = 0; i < dh; ++i)
                {
                    y = yoffs[i];
                    for (int j = 0; j < dw; ++j)
                    {
                        x = xoffs[j];
                        this[dy + i, dx + j] = this[y, x];
                    }
                }
            }
        }

        private void FillBorderReflect(int top, int left, int bottom, int right)
        {
            int h = this.Height - top - bottom;
            int w = this.Width - left - right;
            FillReflect(0, 0, top, this.Width, top, left, h, w);                           // top
            FillReflect(top, 0, h, left, top, left, h, w);                                 // left
            FillReflect(top, this.Width - right, h, right, top, left, h, w);               // right
            FillReflect(this.Height - bottom, 0, bottom, this.Width, top, left, h, w);     // bottom
        }

        private void FillReplicateH(int t, int l, int b, int r, int sy, int channels)
        {
            if (channels > 1)
            {
                for (int y = t; y < b; ++y)
                    for (int x = l; x < r; ++x)
                        for (int c = 0; c < channels; ++c)
                            this[y, x, c] = this[sy, x, c];
            }
            else
            {
                for (int y = t; y < b; ++y)
                    for (int x = l; x < r; ++x)
                        this[y, x] = this[sy, x];
            }
        }

        private void FillReplicateV(int t, int l, int b, int r, int sx, int channels)
        {
            if (channels > 1)
            {
                for (int y = t; y < b; ++y)
                    for (int x = l; x < r; ++x)
                        for (int c = 0; c < channels; ++c)
                            this[y, x, c] = this[y, sx, c];
            }
            else
            {
                for (int y = t; y < b; ++y)
                    for (int x = l; x < r; ++x)
                        this[y, x] = this[y, sx];
            }
        }

        private void FillBorderReplicate(int top, int left, int bottom, int right)
        {
            int channels = data.Rank > 2 ? data.Dimension[2] : 1;
            FillReplicateH(0, left, top, this.Width - right, top, channels);
            FillReplicateH(this.Height - bottom, left, this.Height, this.Width - right, this.Height - bottom - 1, channels);
            FillReplicateV(0, 0, this.Height, left, left, channels);
            FillReplicateV(0, this.Width - right, this.Height, this.Width, this.Width - right - 1, channels);
        }

        private void FillRect(int top, int left, int bottom, int right, T[] color)
        {
            if (color.Length == 1)
            {
                var c = color[0];
                for (int y = top; y < bottom; ++y)
                    for (int x = left; x < right; ++x)
                        this[y, x] = c;
            }
            else
            {
                for (int y = top; y < bottom; ++y)
                    for (int x = left; x < right; ++x)
                        for (int c = 0; c < color.Length; ++c)
                            this[y, x, c] = color[c];
            }
        }

        private void FillBorderConstant(int top, int left, int bottom, int right, params T[] color)
        {
            FillRect(0, left, top, this.Width - right, color);
            FillRect(this.Height - bottom, left, this.Height, this.Width - right, color);
            FillRect(0, 0, this.Height, left, color);
            FillRect(0, this.Width - right, this.Height, this.Width, color);
        }

        public void Dispose()
        {
            data = A<T>.Null;
        }
    }
}
