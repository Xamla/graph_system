using System;
using Xamla.Utilities;

namespace Xamla.Types
{
    public sealed class Measurement
        : IComparable<Measurement>, IComparable
    {
        public double Value { get; set; }
        public string Unit { get; set; }

        public Measurement()
        {
        }

        public Measurement(string unit, double value)
        {
            this.Unit = unit;
            this.Value = value;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.Value, this.Unit);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Measurement;
            if (other == null)
                return false;

            return this.Value == other.Value && this.Unit == other.Unit;
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this.Value, this.Unit ?? string.Empty);
        }

        public int CompareTo(Measurement other)
        {
            if (other == null)
                return 1;

            var delta = string.Compare(this.Unit, other.Unit, StringComparison.Ordinal);
            if (delta != 0)
                return delta;

            return this.Value.CompareTo(other.Value);
        }

        public int CompareTo(object obj)
        {
            if (obj is Measurement)
                return this.CompareTo((Measurement)obj);

            throw new ArgumentException("Object is not the same type as this instance.");
        }
    }
}
