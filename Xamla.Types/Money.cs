using System;
using Xamla.Utilities;

namespace Xamla.Types
{
    public sealed class Money
        : IComparable<Money>, IComparable
    {
        public Decimal Amount { get; set; }
        public string Currency { get; set; }

        public Money()
        {
            this.Currency = string.Empty;
        }

        public Money(string currency, decimal amount)
        {
            this.Currency = currency;
            this.Amount = amount;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.Currency, this.Amount);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Money;
            if (other == null)
                return false;

            return this.Amount == other.Amount && this.Currency.Equals(other.Currency, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this.Amount, this.Currency ?? string.Empty);
        }

        public int CompareTo(Money other)
        {
            if (other == null)
                return 1;

            var delta = string.Compare(this.Currency, other.Currency, StringComparison.OrdinalIgnoreCase);
            if (delta != 0)
                return delta;

            return this.Amount.CompareTo(other.Amount);
        }

        public int CompareTo(object obj)
        {
            if (obj is Money)
                return this.CompareTo((Money)obj);

            throw new ArgumentException("Object is not the same type as this instance.");
        }
    }
}
