using System;
using Xamla.Utilities;

namespace Xamla.Types
{
    public sealed class GeoPosition
        : IComparable<GeoPosition>, IComparable
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public GeoPosition()
        {
        }

        public GeoPosition(double longitude, double latitude)
        {
            this.Longitude = longitude;
            this.Latitude = Latitude;
        }

        public override string ToString()
        {
            return string.Format("{0:F3}, {1:F3}", this.Latitude, this.Longitude);
        }

        public override bool Equals(object obj)
        {
            var other = obj as GeoPosition;
            if (other == null)
                return false;

            return this.Longitude == other.Longitude && this.Latitude == other.Latitude;
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this.Longitude, this.Latitude);
        }

        public int CompareTo(GeoPosition other)
        {
            if (other == null)
                return 1;

            var delta = this.Longitude - other.Longitude;
            if (delta == 0)
            {
                delta = this.Latitude - other.Latitude;
                if (delta == 0)
                    return 0;
            }

            return delta < 0 ? -1 : 1;
        }

        public int CompareTo(object obj)
        {
            if (obj is GeoPosition)
                return this.CompareTo((GeoPosition)obj);

            throw new ArgumentException("Object is not the same type as this instance.");
        }
    }
}
