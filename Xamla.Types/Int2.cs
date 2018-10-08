using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Xamla.Types
{
    //[Serializable]
    public struct Int2
    {
        public static readonly Int2 Zero = new Int2();
        public static readonly Int2 MaxValue = new Int2(int.MaxValue);
        public static readonly Int2 MinValue = new Int2(int.MinValue);

        public int X;
        public int Y;

        public Int2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Int2(int x)
            : this(x, x)
        {
        }

        public int DistanceSquared(Int2 b)
        {
            return (this - b).LengthSquared;
        }

        public double Distance(Int2 b)
        {
            return Math.Sqrt(DistanceSquared(b));
        }

        [JsonIgnore]
        public int LengthSquared
        {
            get { return X * X + Y * Y; }
        }

        [JsonIgnore]
        public double Length
        {
            get { return Math.Sqrt(this.LengthSquared); }
        }

        public Int2 Clip(Int2 topLeft, Int2 bottomRight)
        {
            return new Int2(
                Math.Min(Math.Max(topLeft.X, this.X), bottomRight.X),
                Math.Min(Math.Max(topLeft.Y, this.Y), bottomRight.Y)
            );
        }

        public Int2 Clip(IntRect range)
        {
            return Clip(range.TopLeft, range.BottomRight);
        }

        public IEnumerable<Int2> NeighborValuesWithinRadius(double radius, Int2 size)
        {
            if (radius <= 0)
                yield break;

            int intRadius = (int)Math.Ceiling(radius);

            Int2 topLeft = this - new Int2(intRadius, intRadius);
            Int2 bottomRight = this + new Int2(intRadius, intRadius);

            topLeft = topLeft.Clip(Int2.Zero, size);
            bottomRight = bottomRight.Clip(Int2.Zero, size);

            var radiusSquared = radius * radius;

            for (int y = topLeft.Y; y < bottomRight.Y; ++y)
            {
                // for every line walk in both directions from the horizontal center
                for (int x = this.X; x < bottomRight.X && this.DistanceSquared(new Int2(x, y)) <= radiusSquared; ++x)
                {
                    yield return new Int2(x, y);
                }
                for (int x = this.X - 1; x >= topLeft.X && this.DistanceSquared(new Int2(x, y)) <= radiusSquared; --x)
                {
                    yield return new Int2(x, y);
                }
            }
        }

        public Int2 Abs()
        {
            return new Int2(Math.Abs(X), Math.Abs(Y));
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", X, Y);
        }

        public int[] ToArray()
        {
            return new int[] { X, Y };
        }

        public static Int2 operator -(Int2 a, Int2 b)
        {
            return new Int2(a.X - b.X, a.Y - b.Y);
        }

        public static Int2 operator +(Int2 a, Int2 b)
        {
            return new Int2(a.X + b.X, a.Y + b.Y);
        }

        public static Int2 operator /(Int2 a, int b)
        {
            return new Int2(a.X / b, a.Y / b);
        }

        public static Int2 operator *(Int2 a, int b)
        {
            return new Int2(a.X * b, a.Y * b);
        }

        public static explicit operator Int2(Float2 f)
        {
            return new Int2((int)f.X, (int)f.Y);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static bool operator ==(Int2 a, Int2 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Int2 a, Int2 b)
        {
            return !a.Equals(b);
        }

        public static Int2 Parse(string input)
        {
            Int2 result;
            if (!TryParse(input, out result))
                throw new FormatException("Input string is not in the correct format.");
            return result;
        }

        public static bool TryParse(string input, out Int2 result)
        {
            result = Int2.Zero;
            var parts = input.Split(',');
            return parts.Length == 2
                && int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.X)
                && int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.Y);
        }

        public static Int2 Min(Int2 a, Int2 b)
        {
            return new Int2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        }

        public static Int2 Max(Int2 a, Int2 b)
        {
            return new Int2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        }
    }
}
