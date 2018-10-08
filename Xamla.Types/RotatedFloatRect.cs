using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xamla.Types
{
    [Serializable]
    public struct RotatedFloatRect
    {
        const double degreeToRadian = Math.PI / 180.0;
        const double radianToDegree = 180.0 / Math.PI;

        public static double DegreeToRadian(double degrees)
        {
            return degrees * degreeToRadian;
        }

        public static double RadianToDegree(double radians)
        {
            return radians * radianToDegree;
        }

        Float2 center;
        Float2 size;
        double angle;

        public RotatedFloatRect(Float2 center, Float2 size, double angleRadians)
        {
            this.center = center;
            this.size = size;
            this.angle = angleRadians;
        }

        public RotatedFloatRect(FloatRect rect, double angle = 0)
        {
            center = rect.Center;
            size = rect.Size;
            this.angle = angle;
        }

        public RotatedFloatRect(double cx, double cy, double sx, double sy, double angleRadians)
        {
            center = new Float2(cx, cy);
            size = new Float2(sx, sy);
            this.angle = angleRadians;
        }

        public Float2 Size
        {
            get { return size; }
            set { size = value; }
        }

        public Float2 Center
        {
            get { return center; }
            set { center = value; }
        }

        public double AngleRadians
        {
            get { return angle; }
            set { angle = value; }
        }

        [JsonIgnore]
        public double AngleDegrees
        {
            get { return RadianToDegree(angle); }
            set { angle = DegreeToRadian(value); }
        }

        [JsonIgnore]
        public Float2[] Vertices
        {
            get
            {
                var br = size * 0.5;
                var xs = new FloatRect(-br, br).Vertices;
                var ys = new Float2[4];
                for (int i = 0; i < 4; ++i)
                {
                    var c = Math.Cos(angle);
                    var s = Math.Sin(angle);
                    ys[i].X = xs[i].X * c - xs[i].Y * s + center.X;
                    ys[i].Y = xs[i].X * s + xs[i].Y * c + center.Y;
                }

                return ys;
            }
        }

        [JsonIgnore]
        public FloatRect BoundingFloatRect
        {
            get { return FloatRect.BoundingRect(this.Vertices); }
        }

        public IntRect BoundingIntRect
        {
            get { return IntRect.BoundingRect(this.Vertices); }
        }

        public RotatedFloatRect Offset(double x, double y)
        {
            return Offset(new Float2(x, y));
        }

        public RotatedFloatRect Offset(Float2 offset)
        {
            return new RotatedFloatRect(center + offset, size, angle);
        }

        public RotatedFloatRect Scale(double factor)
        {
            return Scale(factor, factor);
        }

        public RotatedFloatRect Scale(double factorX, double factorY)
        {
            return new RotatedFloatRect(center, size * new Float2(factorX, factorY), angle);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2:G}", center, size, angle);
        }
    }
}
