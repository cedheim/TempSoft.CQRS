using System;
using Microsoft.VisualBasic.CompilerServices;

namespace TempSoft.CQRS.Demo.Domain.Movies.Values
{
    public class Country : IEquatable<Country>, IComparable, IComparable<Country>
    {
        private Country()
        {
        }

        public Country(string countryCode)
        {
            CountryCode = countryCode.ToUpper();
        }

        public string CountryCode { get; private set; }

        public bool Equals(Country other)
        {
            return this.CompareTo(other) == 0;
        }

        public int CompareTo(object obj)
        {
            if (obj is Country other)
            {
                return this.CompareTo(other);
            }

            return -1;
        }

        public int CompareTo(Country other)
        {
            if (object.ReferenceEquals(other, default(Country)))
            {
                return -1;
            }

            return string.Compare(this.CountryCode, other.CountryCode, StringComparison.InvariantCulture);
        }

        public override string ToString()
        {
            return CountryCode;
        }

        public override int GetHashCode()
        {
            return CountryCode.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Country other)
            {
                return this.Equals(other);
            }

            return false;
        }

        public static bool operator ==(Country c1, Country c2)
        {
            return c1?.Equals(c2) ?? false;
        }

        public static bool operator !=(Country c1, Country c2)
        {
            return !(c1 == c2);
        }
    }
}