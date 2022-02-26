using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Appel.SharpTemplate.API.Application.Extensions;

public class JsonConverterExtensions
{
    public class NumbersOnly : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            return reader.TokenType == JsonTokenType.String ? reader.GetString().ToNumbersOnly() : reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
