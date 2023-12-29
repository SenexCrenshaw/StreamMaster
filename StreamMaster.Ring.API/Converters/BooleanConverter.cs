using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Converters;

/// <summary>
/// Converts JSON to a boolean value. Accepts 0, 1, true and false. Non case sensitive comparison.
/// </summary>
public class BooleanConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True:
                return true;
            case JsonTokenType.False:
                return false;
            case JsonTokenType.Number:
                return reader.GetInt32() == 1;
            case JsonTokenType.String:
                return reader.GetString().ToLowerInvariant() switch
                {
                    "true" => true,
                    "1" => true,
                    "false" => false,
                    "0" => false,
                    _ => throw new JsonException()
                };
            default:
                throw new JsonException();
        }
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}