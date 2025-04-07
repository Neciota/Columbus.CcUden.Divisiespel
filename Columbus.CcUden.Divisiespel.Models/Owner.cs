using Columbus.CcUden.Divisiespel.Models.Converters;
using System.Text.Json.Serialization;

namespace Columbus.CcUden.Divisiespel.Models
{
    [JsonConverter(typeof(OwnerConverter))]
    public readonly struct Owner(string name) : IEquatable<Owner>, IComparable<Owner>
    {
        private readonly string _name = name;

        public override string ToString()
        {
            return _name;
        }

        public override bool Equals(object? obj)
        {
            return obj is Owner owner && Equals(owner);
        }

        public bool Equals(Owner other)
        {
            return _name == other._name;
        }

        public override int GetHashCode()
        {
            HashCode hashCode = new();
            hashCode.Add(_name);
            return hashCode.ToHashCode();
        }

        public int CompareTo(Owner other)
        {
            return _name.CompareTo(other._name);
        }

        public static bool operator ==(Owner left, Owner right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Owner left, Owner right)
        {
            return !(left == right);
        }

        public static bool operator <(Owner left, Owner right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Owner left, Owner right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Owner left, Owner right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Owner left, Owner right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
