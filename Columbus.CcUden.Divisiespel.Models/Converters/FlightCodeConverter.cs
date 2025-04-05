using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Columbus.CcUden.Divisiespel.Models.Converters
{
    public class FlightCodeConverter : JsonConverter<FlightCode>
    {
        public override FlightCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return new FlightCode(value ?? string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, FlightCode value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        public override FlightCode ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new FlightCode(reader.GetString() ?? string.Empty);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, [DisallowNull] FlightCode value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(value.ToString());
        }
    }
}
