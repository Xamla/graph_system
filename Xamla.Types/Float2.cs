using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Xamla.Types
{
    //[Serializable]
    public struct Float2
    {
        public static readonly Float2 Zero = new Float2();
        public static readonly Float2 MinValue = new Float2(double.MinValue);
        public static readonly Float2 MaxValue = new Float2(double.MaxValue);

        public double X;
        public double Y;

        public const double Inaccuracy = 0.0001;

        public Float2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Float2(Int2 xy)
        {
            X = xy.X;
            Y = xy.Y;
        }

        public Float2(double x)
            : this(x, x)
        {
        }

        public double DistanceSquared(Float2 b)
        {
            return (this - b).LengthSquared;
        }

        public double Distance(Float2 b)
        {
            return Math.Sqrt(DistanceSquared(b));
        }

        [JsonIgnore]
        public double LengthSquared
        {
            get { return X * X + Y * Y; }
        }

        public double LNorm(double l)
        {
            if (l == 1)
                return X + Y;
            return Math.Pow(Math.Pow(X, l) + Math.Pow(Y, l), 1 / l);
        }

        [JsonIgnore]
        public double Length
        {
            get { return Math.Sqrt(this.LengthSquared); }
        }

        public Float2 Normalize()
        {
            double length = this.Length;
            if (length == 0)
                return this;
            return this / length;
        }

        public Float2 Clip(Float2 topLeft, Float2 bottomRight)
        {
            return new Float2(
                Math.Min(Math.Max(topLeft.X, this.X), bottomRight.X),
                Math.Min(Math.Max(topLeft.Y, this.Y), bottomRight.Y)
            );
        }

        public Float2 Clip(FloatRect range)
        {
            return Clip(range.TopLeft, range.BottomRight);
        }

        public Float2 Round()
        {
            return new Float2(Math.Round(this.X), Math.Round(this.Y));
        }

        public Float2 Ceiling()
        {
            return new Float2(Math.Ceiling(this.X), Math.Ceiling(this.Y));
        }

        public Float2 Floor()
        {
            return new Float2(Math.Floor(this.X), Math.Floor(this.Y));
        }

        public Float2 Abs()
        {
            return new Float2(Math.Abs(X), Math.Abs(Y));
        }

        public Float2 Sqrt()
        {
            return new Float2(Math.Sqrt(X), Math.Sqrt(Y));
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:G}, {1:G}", X, Y);
        }

        public double[] ToArray()
        {
            return new double[] { X, Y };
        }

        public Float2 Rotate(double radians)
        {
            return new Float2(X * Math.Cos(radians) - Y * Math.Sin(radians), Y * Math.Cos(radians) + X * Math.Sin(radians));
        }

        public static Float2 DirectionFromRadians(double radians)
        {
            return new Float2(Math.Cos(radians), Math.Sin(radians));
        }

        public static Float2 CenterOfGravity(IEnumerable<Float2> values)
        {
            return values.Aggregate(new Float2(), (acc, val) => acc + val) / values.Count();
        }

        public double Dot(Float2 a)
        {
            return X * a.X + Y * a.Y;
        }

        public Float2 Perpendicular()
        {
            return new Float2(Y, -X);
        }

        public Float2 Negate()
        {
            return new Float2(-X, -Y);
        }

        public static Float2 operator -(Float2 a)
        {
            return a.Negate();
        }

        public static Float2 operator -(Float2 a, Float2 b)
        {
            return new Float2(a.X - b.X, a.Y - b.Y);
        }

        public static Float2 operator +(Float2 a, Float2 b)
        {
            return new Float2(a.X + b.X, a.Y + b.Y);
        }

        public static Float2 operator *(Float2 a, Float2 b)
        {
            return new Float2(a.X * b.X, a.Y * b.Y);
        }

        public static Float2 operator /(Float2 a, double b)
        {
            return new Float2(a.X / b, a.Y / b);
        }

        public static Float2 operator *(Float2 a, double b)
        {
            return new Float2(a.X * b, a.Y * b);
        }

        public static explicit operator Float2(Int2 i)
        {
            return new Float2(i.X, i.Y);
        }

        public static Float2 Parse(string input)
        {
            Float2 result;
            if (!TryParse(input, out result))
                throw new FormatException("Input string is not in the correct format.");
            return result;
        }

        public static bool TryParse(string input, out Float2 result)
        {
            result = Float2.Zero;
            var parts = input.Split(',');
            return parts.Length == 2
                && double.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out result.X)
                && double.TryParse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out result.Y);
        }

        public static Float2 Min(Float2 a, Float2 b)
        {
            return new Float2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        }

        public static Float2 Max(Float2 a, Float2 b)
        {
            return new Float2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        }
    }
}
