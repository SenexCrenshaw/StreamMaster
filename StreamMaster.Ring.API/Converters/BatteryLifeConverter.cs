using System.Text.Json;
using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Converters;

/// <summary>
/// Converts JSON to from a numeric of string value to a numeric value
/// </summary>
public class BatteryLifeConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                return reader.GetInt32();

            case JsonTokenType.String:
                {
                    string? stringValue = reader.GetString();
                    return int.TryParse(stringValue, out int result) ? result : null;
                }
            default:
                return null;
        }
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(value.Value);
        }

        writer.WriteNullValue();
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return true;
    }
}

public class BatteryLifeConverterLong : JsonConverter<long?>
{
    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                return reader.GetInt64();

            case JsonTokenType.String:
                {
                    string? stringValue = reader.GetString();
                    return long.TryParse(stringValue, out long result) ? result : null;
                }
            default:
                return null;
        }
    }

    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(value.Value);
        }

        writer.WriteNullValue();
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return true;
    }
}