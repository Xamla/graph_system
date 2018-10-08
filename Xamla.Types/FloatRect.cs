using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Xamla.Types
{
    //[Serializable]
    public struct FloatRect
    {
        public static readonly FloatRect Empty = default(FloatRect);

        double left, top, right, bottom;

        public FloatRect(double left, double top, double right, double bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public FloatRect(Float2 topLeft, Float2 bottomRight)
            : this(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y)
        {
        }

        public FloatRect(double[] array)
            : this(array[0], array[1], array[2], array[3])
        {
        }

        public double Left
        {
            get { return left; }
            set { left = value; }
        }

        public double Top
        {
            get { return top; }
            set { top = value; }
        }

        public double Right
        {
            get { return right; }
            set { right = value; }
        }

        public double Bottom
        {
            get { return bottom; }
            set { bottom = value; }
        }

        [JsonIgnore]
        public Float2 Size
        {
            get { return this.BottomRight - this.TopLeft; }
        }

        [JsonIgnore]
        public double Width
        {
            get { return right - left; }
        }

        [JsonIgnore]
        public double Height
        {
            get { return bottom - top; }
        }

        [JsonIgnore]
        public double Area
        {
            get { return this.Width * this.Height; }
        }

        [JsonIgnore]
        public Float2 Center
        {
            get { return new Float2((left + right) / 2.0, (top + bottom) / 2.0); }
        }

        [JsonIgnore]
        public bool IsEmpty
        {
            get { return top == bottom && left == right; }
        }

        [JsonIgnore]
        public Float2 TopLeft
        {
            get { return new Float2(left, top); }
            set
            {
                left = value.X;
                top = value.Y;
            }
        }

        [JsonIgnore]
        public Float2 TopRight
        {
            get { return new Float2(right, top); }
            set
            {
                right = value.X;
                top = value.Y;
            }
        }

        [JsonIgnore]
        public Float2 BottomRight
        {
            get { return new Float2(right, bottom); }
            set
            {
                right = value.X;
                bottom = value.Y;
            }
        }

        [JsonIgnore]
        public Float2 BottomLeft
        {
            get { return new Float2(left, bottom); }
            set
            {
                left = value.X;
                bottom = value.Y;
            }
        }

        [JsonIgnore]
        public Float2[] Vertices
        {
            get { return new Float2[] { this.TopLeft, this.TopRight, this.BottomRight, this.BottomLeft }; }
        }

        public FloatRect Offset(double x, double y)
        {
            return new FloatRect(left + x, top + y, right + x, bottom + y);
        }

        public FloatRect Offset(Float2 offset)
        {
            return Offset(offset.X, offset.Y);
        }

        public FloatRect Normalize()
        {
            return new FloatRect(
                (left <= right) ? left : right,
                (top <= bottom) ? top : bottom,
                (right > left) ? right : left,
                (bottom > top) ? bottom : top
            );
        }

        public FloatRect Clip(FloatRect clipArea)
        {
            return new FloatRect(
                Math.Min(Math.Max(left, clipArea.left), clipArea.right),
                Math.Min(Math.Max(top, clipArea.top), clipArea.bottom),
                Math.Max(Math.Min(right, clipArea.right), clipArea.left),
                Math.Max(Math.Min(bottom, clipArea.bottom), clipArea.top)
            );
        }

        public bool Contains(double x, double y)
        {
            return left <= x && x < right && this.top <= y && y < bottom;
        }

        public bool Contains(Float2 p)
        {
            return Contains(p.X, p.Y);
        }

        public bool IntersectsWith(FloatRect rect)
        {
            return this.left < rect.right && rect.left < this.right && this.top < rect.bottom && rect.top < this.bottom;
        }

        public FloatRect Inflate(double x, double y)
        {
            return new FloatRect(left - x, top - y, right + x, bottom + y);
        }

        public FloatRect Inflate(Float2 size)
        {
            return Inflate(size.X, size.Y);
        }

        public FloatRect Union(FloatRect other)
        {
            return Union(this, other);
        }

        public FloatRect Intersect(FloatRect other)
        {
            return Intersect(this, other);
        }

        public static FloatRect Union(FloatRect a, FloatRect b)
        {
            var left = Math.Min(a.left, b.left);
            var top = Math.Min(a.top, b.top);
            var right = Math.Max(a.right, b.right);
            var bottom = Math.Max(a.bottom, b.bottom);
            return new FloatRect(left, top, right, bottom);
        }

        public static FloatRect Intersect(FloatRect a, FloatRect b)
        {
            var left = Math.Max(a.left, b.left);
            var top = Math.Max(a.top, b.top);
            var right = Math.Min(a.right, b.right);
            var bottom = Math.Min(a.bottom, b.bottom);
            return right >= left && bottom >= top ? new FloatRect(left, top, right, bottom) : FloatRect.Empty;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:G}, {1:G}, {2:G}, {3:G}", left, top, right, bottom);
        }

        public IntRect ToIntRect()
        {
            return new IntRect((int)left, (int)top, (int)right, (int)bottom);
        }

        public double[] ToArray()
        {
            return new double[] { left, top, right, bottom };
        }

        public Float2[] ToFloat2Array()
        {
            return new Float2[] { this.TopLeft, this.BottomRight };
        }

        public static FloatRect FromLeftTopWidthHeight(Float2 leftTop, Float2 size)
        {
            return new FloatRect(leftTop, leftTop + size);
        }

        public static FloatRect FromLeftTopWidthHeight(double left, double top, double width, double height)
        {
            return new FloatRect(left, top, left + width, top + height);
        }

        public static FloatRect Parse(string input)
        {
            FloatRect result;
            if (!TryParse(input, out result))
                throw new FormatException("Input string is not in the correct format.");
            return result;
        }

        public static bool TryParse(string input, out FloatRect result)
        {
            result = default(FloatRect);
            var parts = input.Split(',');
            return parts.Length == 4
                && double.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out result.left)
                && double.TryParse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out result.top)
                && double.TryParse(parts[2], NumberStyles.Number, CultureInfo.InvariantCulture, out result.right)
                && double.TryParse(parts[3], NumberStyles.Number, CultureInfo.InvariantCulture, out result.bottom);
        }

        public static FloatRect RandomFloatRect(Random rng, FloatRect sampleRegion)
        {
            double w = sampleRegion.Width, h = sampleRegion.Height;
            var tl = new Float2(rng.NextDouble() * w, rng.NextDouble() * h);
            var sz = new Float2(rng.NextDouble() * (w - tl.X), rng.NextDouble() * (h - tl.Y));
            return FromLeftTopWidthHeight(tl, sz).Normalize();
        }

        public static IEnumerable<FloatRect> RandomFloatRectSequence(Random rng, FloatRect sampleRegion)
        {
            for (; ; )
                yield return RandomFloatRect(rng, sampleRegion);
        }

        public static FloatRect RandomFloatRect(Random rng, FloatRect sampleRegion, Float2 minSize, Float2 maxSize)
        {
            double w = sampleRegion.Width, h = sampleRegion.Height;
            var p1 = new Float2(rng.NextDouble() * (w - minSize.X), rng.NextDouble() * (h - minSize.Y));

            if (minSize.Equals(maxSize))
            {
                return FromLeftTopWidthHeight(p1, maxSize);
            }
            else
            {
                var sz = new Float2(
                    rng.NextDouble() * (Math.Min(w - p1.X, maxSize.X)),
                    rng.NextDouble() * (Math.Min(h - p1.Y, maxSize.Y))
                );

                return FromLeftTopWidthHeight(p1, sz);
            }
        }

        public static IEnumerable<FloatRect> RandomFloatRectSequence(Random rng, FloatRect sampleRegion, Float2 minSize, Float2 maxSize, IEnumerable<FloatRect> excluded = null)
        {
            for (; ; )
            {
                var r = RandomFloatRect(rng, sampleRegion, minSize, maxSize);

                if (excluded != null)
                {
                    if (excluded.Any(x => x.IntersectsWith(r)))
                        continue;
                }

                yield return r;
            }
        }

        public static FloatRect BoundingRect(IEnumerable<Float2> points)
        {
            var box = new FloatRect(double.MaxValue, double.MaxValue, double.MinValue, double.MinValue);

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

            return box.left > box.Right ? FloatRect.Empty : box;
        }

        public static FloatRect BoundingRect(IEnumerable<FloatRect> rects)
        {
            return BoundingRect(rects.SelectMany(x => x.ToFloat2Array()));
        }
    }
}
