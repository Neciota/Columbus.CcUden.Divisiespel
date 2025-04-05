using Columbus.CcUden.Divisiespel.Models.Converters;
using System.Text.Json.Serialization;

namespace Columbus.CcUden.Divisiespel.Models
{
    [JsonConverter(typeof(FlightCodeConverter))]
    public readonly struct FlightCode : IComparable<FlightCode>, IEquatable<FlightCode>
    {
        private readonly string _code;

        public FlightCode(string code)
        {
            if (!int.TryParse(code[1..], out int _))
                throw new ArgumentException("This is not a valid flight code. Only the first letter is allowed to be a non-number.");

            _code = code;
        }

        public readonly int GetNumber() => Convert.ToInt32(_code[1..]);

        public int CompareTo(FlightCode other)
        {
            return GetNumber().CompareTo(other.GetNumber());
        }

        public override string ToString()
        {
            return _code;
        }

        public override bool Equals(object? obj)
        {
            return obj is FlightCode flightCode && Equals(flightCode);
        }

        public bool Equals(FlightCode other)
        {
            return _code == other._code;
        }

        public override int GetHashCode()
        {
            HashCode hashCode = new();
            hashCode.Add(_code);
            return hashCode.ToHashCode();
        }

        public static bool operator ==(FlightCode left, FlightCode right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FlightCode left, FlightCode right)
        {
            return !(left == right);
        }

        public static bool operator <(FlightCode left, FlightCode right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(FlightCode left, FlightCode right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(FlightCode left, FlightCode right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(FlightCode left, FlightCode right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
