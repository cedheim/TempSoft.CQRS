using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TempSoft.CQRS.Demo.ValueObjects
{
    public class Name : IComparable, IComparable<Name>, IEquatable<Name>
    {
        public string FirstName { get; private set; }
        public string MiddleName { get; private set; }
        public string LastName { get; private set; }
        public string Identifier { get; private set; }

        private Name()
        {
        }

        public Name(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            Identifier = GenerateIdentifier();
        }

        [JsonConstructor]
        public Name(string firstName, string middleName, string lastName)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Identifier = GenerateIdentifier();
        }

        private string GenerateIdentifier()
        {
            return Names().Aggregate(string.Empty, (s, s1) => string.IsNullOrEmpty(s) ? s1 : $"{s}_{s1}").ToLower();
        }

        private IEnumerable<string> Names()
        {
            if (!string.IsNullOrEmpty(LastName))
                yield return LastName;
            if (!string.IsNullOrEmpty(MiddleName))
                yield return MiddleName;
            if (!string.IsNullOrEmpty(FirstName))
                yield return FirstName;


        }

        public int CompareTo(object obj)
        {
            if (obj is Name other)
            {
                return CompareTo(other);
            }

            return -1;
        }

        public int CompareTo(Name other)
        {
            if (object.ReferenceEquals(other, default(Name)))
            {
                return -1;
            }

            return string.Compare(Identifier, other.Identifier, StringComparison.InvariantCulture);
        }

        public bool Equals(Name other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is Name other)
            {
                return Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public override string ToString()
        {
            return Identifier;
        }

        public static bool operator ==(Name n1, Name n2)
        {
            return n1?.Equals(n2) ?? false;
        }

        public static bool operator !=(Name n1, Name n2)
        {
            return !(n1 == n2);
        }
    }
}