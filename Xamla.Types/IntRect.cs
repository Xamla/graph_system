    using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Xamla.Types
{
    //[Serializable]
    public struct IntRect
    {
        public static readonly IntRect Empty = default(IntRect);

        int left, top, right, bottom;

        public IntRect(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public IntRect(Int2 topLeft, Int2 bottomRight)
            : this(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y)
        {
        }

        public IntRect(int[] array)
            : this(array[0], array[1], array[2], array[3])
        {
        }

        public int Left
        {
            get { return left; }
            set { left = value; }
        }

        public int Top
        {
            get { return top; }
            set { top = value; }
        }

        public int Right
        {
            get { return right; }
            set { right = value; }
        }

        public int Bottom
        {
            get { return bottom; }
            set { bottom = value; }
        }

        [JsonIgnore]
        public Int2 Size
        {
            get { return this.BottomRight - this.TopLeft; }
        }

        [JsonIgnore]
        public int Width
        {
            get { return right - left; }
        }

        [JsonIgnore]
        public int Height
        {
            get { return bottom - top; }
        }

        [JsonIgnore]
        public int Area
        {
            get { return this.Width * this.Height; }
        }

        [JsonIgnore]
        public Int2 Center
        {
            get { return new Int2((left + right) / 2, (top + bottom) / 2); }
        }

        [JsonIgnore]
        public bool IsEmpty
        {
            get { return top == bottom && left == right; }
        }

        [JsonIgnore]
        public Int2 TopLeft
        {
            get { return new Int2(left, top); }
            set
            {
                left = value.X;
                top = value.Y;
            }
        }

        [JsonIgnore]
        public Int2 TopRight
        {
            get { return new Int2(right, top); }
            set
            {
                right = value.X;
                top = value.Y;
            }
        }

        [JsonIgnore]
        public Int2 BottomRight
        {
            get { return new Int2(right, bottom); }
            set
            {
                right = value.X;
                bottom = value.Y;
            }
        }

        [JsonIgnore]
        public Int2 BottomLeft
        {
            get { return new Int2(left, bottom); }
            set
            {
                left = value.X;
                bottom = value.Y;
            }
        }

        [JsonIgnore]
        public Int2[] Verticies
        {
            get { return new Int2[] { this.TopLeft, this.TopRight, this.BottomRight, this.BottomLeft }; }
        }

        public IntRect Offset(int x, int y)
        {
            return new IntRect(left + x, top + y, right + x, bottom + y);
        }

        public IntRect Offset(Int2 offset)
        {
            return Offset(offset.X, offset.Y);
        }

        public IntRect Normalize()
        {
            return new IntRect(
                (left <= right) ? left : right,
                (top <= bottom) ? top : bottom,
                (right > left) ? right : left,
                (bottom > top) ? bottom : top
            );
        }

        public IntRect Clip(IntRect clipArea)
        {
            return new IntRect(
                Math.Min(Math.Max(left, clipArea.left), clipArea.right),
                Math.Min(Math.Max(top, clipArea.top), clipArea.bottom),
                Math.Max(Math.Min(right, clipArea.right), clipArea.left),
                Math.Max(Math.Min(bottom, clipArea.bottom), clipArea.top)
            );
        }

        public bool Contains(int x, int y)
        {
            return left <= x && x < right && this.top <= y && y < bottom;
        }

        public bool Contains(Int2 p)
        {
            return Contains(p.X, p.Y);
        }

        public bool IntersectsWith(IntRect rect)
        {
            return this.left < rect.right && rect.left < this.right && this.top < rect.bottom && rect.top < this.bottom;
        }

        public IntRect Inflate(int x, int y)
        {
            return new IntRect(left - x, top - y, right + x, bottom + y);
        }

        public IntRect Inflate(Int2 size)
        {
            return Inflate(size.X, size.Y);
        }

        public IntRect Union(IntRect other)
        {
            return Union(this, other);
        }

        public IntRect Intersect(IntRect other)
        {
            return Intersect(this, other);
        }

        public static IntRect Union(IntRect a, IntRect b)
        {
            var left = Math.Min(a.left, b.left);
            var top = Math.Min(a.top, b.top);
            var right = Math.Max(a.right, b.right);
            var bottom = Math.Max(a.bottom, b.bottom);
            return new IntRect(left, top, right, bottom);
        }

        public static IntRect Intersect(IntRect a, IntRect b)
        {
            var left = Math.Max(a.left, b.left);
            var top = Math.Max(a.top, b.top);
            var right = Math.Min(a.right, b.right);
            var bottom = Math.Min(a.bottom, b.bottom);
            return right >= left && bottom >= top ? new IntRect(left, top, right, bottom) : IntRect.Empty;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", left, top, right, bottom);
        }

        public FloatRect ToFloatRect()
        {
            return new FloatRect(left, top, right, bottom);
        }

        public int[] ToArray()
        {
            return new int[] { left, top, right, bottom };
        }

        public Int2[] ToInt2Array()
        {
            return new Int2[] { this.TopLeft, this.BottomRight };
        }

        public static IntRect FromLeftTopWidthHeight(Int2 leftTop, Int2 size)
        {
            return new IntRect(leftTop, leftTop + size);
        }

        public static IntRect FromLeftTopWidthHeight(int left, int top, int width, int height)
        {
            return new IntRect(left, top, left + width, top + height);
        }

        public static IntRect Parse(string input)
        {
            IntRect result;
            if (!TryParse(input, out result))
                throw new FormatException("Input string is not in the correct format.");
            return result;
        }

        public static bool TryParse(string input, out IntRect result)
        {
            result = default(IntRect);
            var parts = input.Split(',');
            return parts.Length == 4
                && int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.left)
                && int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.top)
                && int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.right)
                && int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.bottom);
        }

        public static IntRect RandomIntRect(Random rng, IntRect sampleRegion)
        {
            int w = sampleRegion.Width, h = sampleRegion.Height;
            var tl = new Int2(rng.Next(w), rng.Next(h));
            var sz = new Int2(rng.Next(w - tl.X), rng.Next(h - tl.Y));
            return new IntRect(tl, sz).Normalize();
        }

        public static IEnumerable<IntRect> RandomIntRectSequence(Random rng, IntRect sampleRegion)
        {
            for (; ; )
                yield return RandomIntRect(rng, sampleRegion);
        }

        public static IntRect RandomIntRect(Random rng, IntRect sampleRegion, Int2 minSize, Int2 maxSize)
        {
            int w = sampleRegion.Width, h = sampleRegion.Height;
            var tl = new Int2(rng.Next(w - minSize.X), rng.Next(h - minSize.Y));

            if (minSize == maxSize)
            {
                return FromLeftTopWidthHeight(tl, maxSize);
            }
            else
            {
                var sz = new Int2(
                    rng.Next(Math.Min(w - tl.X, maxSize.X)),
                    rng.Next(Math.Min(h - tl.Y, maxSize.Y))
                );
                return FromLeftTopWidthHeight(tl, sz);
            }
        }

        public static IEnumerable<IntRect> RandomIntRectSequence(Random rng, IntRect sampleRegion, Int2 minSize, Int2 maxSize, IEnumerable<IntRect> excluded = null)
        {
            for (; ; )
            {
                var r = RandomIntRect(rng, sampleRegion, minSize, maxSize);

                if (excluded != null)
                {
                    if (excluded.Any(x => x.IntersectsWith(r)))
                        continue;
                }

                yield return r;
            }
        }

        public static IntRect BoundingRect(IEnumerable<Int2> points)
        {
            var box = new IntRect(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);

            foreach (var p in points)
            {
                if (p.X > box.Right)
                    box.Right = p.X;
                if (p.X < box.Left)
                    box.Left = p.X;
                if (p.Y > box.Bottom)
                    box.Bottom = p.Y;
                if (p.Y < box.Top)
                    box.Top = p.Y;
            }

            return box.left > box.Right ? IntRect.Empty : box;
        }

        public static IntRect BoundingRect(IEnumerable<IntRect> rects)
        {
            return BoundingRect(rects.SelectMany(x => x.ToInt2Array()));
        }

        public static IntRect BoundingRect(IEnumerable<Float2> points)
        {
            var box = new IntRect(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);

            foreach (var p in points)
            {
                if (p.X > box.Right)
                    box.Right = (int)Math.Ceiling(p.X);
                if (p.X < box.Left)
                    box.Left = (int)Math.Floor(p.X);
                if (p.Y > box.Bottom)
                    box.Bottom = (int)Math.Ceiling(p.Y);
                if (p.Y < box.Top)
                    box.Top = (int)Math.Floor(p.Y);
            }

            return box.left > box.Right ? IntRect.Empty : box;
        }
    }
}
