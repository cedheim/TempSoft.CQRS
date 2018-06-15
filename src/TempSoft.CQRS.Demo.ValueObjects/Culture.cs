using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TempSoft.CQRS.Demo.ValueObjects
{
    public class Culture : IEquatable<Culture>, IComparable, IComparable<Culture>
    {
        private Culture()
        {
        }
        
        [JsonConstructor]
        public Culture(string country, string language)
        {
            Country = country.ToUpper().Trim();
            Language = language.ToLower().Trim();

            Identifier = GenerateIdentifier();
        }

        public Culture(string culture)
        {

            var dashIndex = culture.IndexOf('-');
            if (dashIndex < 0)
            {
                Country = culture.ToUpper().Trim();
            }
            else
            {
                Language = culture.Substring(0, dashIndex).ToLower().Trim();
                Country = culture.Substring(dashIndex + 1).ToUpper().Trim();
            }

            Identifier = GenerateIdentifier();
        }

        public string Language { get; private set; }

        public string Country { get; private set; }

        public string Identifier { get; private set; }

        public bool Equals(Culture other)
        {
            return this.CompareTo(other) == 0;
        }

        public int CompareTo(object obj)
        {
            if (obj is Culture other)
            {
                return this.CompareTo(other);
            }

            return -1;
        }

        public int CompareTo(Culture other)
        {
            if (object.ReferenceEquals(other, default(Culture)))
            {
                return -1;
            }

            return string.Compare(this.Country, other.Country, StringComparison.InvariantCulture);
        }

        public override string ToString()
        {
            return Identifier;
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Culture other)
            {
                return this.Equals(other);
            }

            return false;
        }

        public static bool operator ==(Culture c1, Culture c2)
        {
            return c1?.Equals(c2) ?? false;
        }

        public static bool operator !=(Culture c1, Culture c2)
        {
            return !(c1 == c2);
        }

        private string GenerateIdentifier()
        {
            return GetCultureComponents().Aggregate(string.Empty, (s, s1) => string.IsNullOrEmpty(s) ? s1 : $"{s}-{s1}");
        }

        private IEnumerable<string> GetCultureComponents()
        {
            if (!string.IsNullOrEmpty(Language))
                yield return Language;
            if (!string.IsNullOrEmpty(Country))
                yield return Country;
        }
    }
}