using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xamla.Types
{
    //[Serializable]
    public struct Float3
    {
        public static readonly Float3 Zero = new Float3();
        public static readonly Float3 MinValue = new Float3(double.MinValue);
        public static readonly Float3 MaxValue = new Float3(double.MaxValue);

        public double X;
        public double Y;
        public double Z;

        public const double Inaccuracy = 0.0001;

        public Float3(double x, double y, double z)
        {
           X = x;
           Y = y;
           Z = z;
        }

        public Float3(Int3 i)
        {
            X = i.X;
            Y = i.Y;
            Z = i.Z;
        }

        public Float3(double x)
            : this(x, x, x)
        {
        }

        public double DistanceSquared(Float3 b)
        {
            return (this - b).LengthSquared;
        }

        public double Distance(Float3 b)
        {
            return Math.Sqrt(DistanceSquared(b));
        }

        [JsonIgnore]
        public double LengthSquared
        {
            get { return X * X + Y * Y + Z * Z; }
        }

        public double LNorm(double l)
        {
            if (l == 1)
                return X + Y + Z;
            return Math.Pow(Math.Pow(X, l) + Math.Pow(Y, l) + Math.Pow(Z, l), 1 / l);
        }

        [JsonIgnore]
        public double Length
        {
            get { return Math.Sqrt(this.LengthSquared); }
        }

        public Float3 Normalize()
        {
            double length = this.Length;
            if (length == 0)
                return this;
            return this / length;
        }

        public Float3 Clip(double minX, double maxX, double minY, double maxY, double minZ, double maxZ)
        {
            return Clip(new Float3(minX, minY, minZ), new Float3(maxX, maxY, maxZ));
        }

        public Float3 Clip(Float3 low, Float3 high)
        {
            return new Float3(
                Math.Min(Math.Max(low.X, this.X), high.X),
                Math.Min(Math.Max(low.Y, this.Y), high.Y),
                Math.Min(Math.Max(low.Z, this.Z), high.Z)
            );
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

        public Float3 Abs()
        {
            return new Float3(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
        }

        public Float3 Sqrt()
        {
            return new Float3(Math.Sqrt(X), Math.Sqrt(Y), Math.Sqrt(Z));
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:G}, {1:G}, {1:G}", X, Y, Z);
        }

        public double[] ToArray()
        {
            return new double[] { X, Y, Z };
        }

        public static Float3 CenterOfGravity(IEnumerable<Float3> values)
        {
            return values.Aggregate(new Float3(), (acc, val) => acc + val) / values.Count();
        }

        public double Dot(Float3 a)
        {
            return X * a.X + Y * a.Y + Z * a.Z;
        }

        public static Float3 operator -(Float3 a, Float3 b)
        {
            return new Float3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Float3 operator +(Float3 a, Float3 b)
        {
            return new Float3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Float3 operator *(Float3 a, Float3 b)
        {
            return new Float3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Float3 operator /(Float3 a, double b)
        {
            return new Float3(a.X / b, a.Y / b, a.Z / b);
        }

        public static Float3 operator *(Float3 a, double b)
        {
            return new Float3(a.X * b, a.Y * b, a.Z * b);
        }

        public static explicit operator Float3(Int3 i)
        {
            return new Float3(i.X, i.Y, i.Z);
        }

        public static Float3 Parse(string input)
        {
            Float3 result;
            if (!TryParse(input, out result))
                throw new FormatException("Input string is not in the correct format.");
            return result;
        }

        public static bool TryParse(string input, out Float3 result)
        {
            result = Float3.Zero;
            var parts = input.Split(',');
            return parts.Length == 3
                && double.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out result.X)
                && double.TryParse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out result.Y)
                && double.TryParse(parts[2], NumberStyles.Number, CultureInfo.InvariantCulture, out result.Z);
        }

        public static Float3 Min(Float3 a, Float3 b)
        {
            return new Float3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        }

        public static Float3 Max(Float3 a, Float3 b)
        {
            return new Float3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
        }
    }
}
