using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Columbus.CcUden.Divisiespel.Models.Converters
{
    public class OwnerConverter : JsonConverter<Owner>
    {
        public override Owner Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return new Owner(value ?? string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, Owner value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        public override Owner ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new Owner(reader.GetString() ?? string.Empty);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, [DisallowNull] Owner value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(value.ToString());
        }
    }
}
