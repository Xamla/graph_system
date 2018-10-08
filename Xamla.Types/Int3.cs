using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Xamla.Types
{
    //[Serializable]
    public struct Int3
    {
        public static readonly Int3 Zero = new Int3();
        public static readonly Int3 MaxValue = new Int3(int.MaxValue);
        public static readonly Int3 MinValue = new Int3(int.MinValue);

        public int X;
        public int Y;
        public int Z;

        public Int3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Int3(int x)
            : this(x, x, x)
        {
        }

        public int DistanceSquared(Int3 b)
        {
            return (this - b).LengthSquared;
        }

        public double Distance(Int3 b)
        {
            return Math.Sqrt(DistanceSquared(b));
        }

        [JsonIgnore]
        public int LengthSquared
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

        public Int3 Clip(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
        {
            return Clip(new Int3(minX, minY, minZ), new Int3(maxX, maxY, maxZ));
        }

        public Int3 Clip(Int3 low, Int3 high)
        {
            return new Int3(
                Math.Min(Math.Max(low.X, this.X), high.X),
                Math.Min(Math.Max(low.Y, this.Y), high.Y),
                Math.Min(Math.Max(low.Z, this.Z), high.Z)
            );
        }

        public Int3 Abs()
        {
            return new Int3(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", X, Y, Z);
        }

        public int[] ToArray()
        {
            return new int[] { X, Y, Z };
        }

        public static Int3 operator -(Int3 a, Int3 b)
        {
            return new Int3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Int3 operator +(Int3 a, Int3 b)
        {
            return new Int3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Int3 operator /(Int3 a, int b)
        {
            return new Int3(a.X / b, a.Y / b, a.Z / b);
        }

        public static Int3 operator *(Int3 a, int b)
        {
            return new Int3(a.X * b, a.Y * b, a.Z * b);
        }

        public static explicit operator Int3(Float3 f)
        {
            return new Int3((int)f.X, (int)f.Y, (int)f.Z);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static bool operator==(Int3 a, Int3 b)
        {
            return a.Equals(b);
        }

        public static bool operator!=(Int3 a, Int3 b)
        {
            return !a.Equals(b);
        }

        public static Int3 Parse(string input)
        {
            Int3 result;
            if (!TryParse(input, out result))
                throw new FormatException("Input string is not in the correct format.");
            return result;
        }

        public static bool TryParse(string input, out Int3 result)
        {
            result = Int3.Zero;
            var parts = input.Split(',');
            return parts.Length == 3
                && int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.X)
                && int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.Y)
                && int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.Z);
        }

        public static Int3 Min(Int3 a, Int3 b)
        {
            return new Int3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        }

        public static Int3 Max(Int3 a, Int3 b)
        {
            return new Int3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
        }
    }
}
