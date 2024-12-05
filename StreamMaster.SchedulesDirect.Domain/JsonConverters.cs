using System.Text.Json;
using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain;

public class IntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return 0;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                if (int.TryParse(reader.GetString(), out int intValue))
                {
                    return intValue;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out int intValue))
                {
                    return intValue;
                }
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Handle JSON object when the TokenType is StartObject
                // Assuming "year" is a property within this object
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "year")
                    {
                        reader.Read(); // Move to the property value

                        if (reader.TryGetInt32(out int yearValue))
                        {
                            reader.Read();
                            return yearValue;
                        }
                        else
                        {
                            // Handle the case where the "year" property is not a valid integer
                            // You can return a default value or throw an exception as needed
                            return 0; // Default value or another meaningful value
                        }
                    }
                }
            }
        }
        catch (JsonException)
        {
            // Handle bad values by returning a default value or throwing a custom exception if needed

        }

        // Return a default value for bad or unexpected data
        return 0;
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("year");
        JsonSerializer.Serialize(writer, value, options);
        writer.WriteEndObject();
    }
}

//internal class IntConverter : JsonConverter
//{

//    public override bool CanConvert(Type typeToConvert)
//    {
//        return Int32.TryParse(typeToConvert.ToString(), out _);
//    }

//    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonIndentOptions options)
//    {
//        return Int32.Parse(reader.GetString());
//    }
//}
