using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StreamMaster.SchedulesDirectAPI.Domain;


public class IntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
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

        // Handle other cases or throw an exception if needed
        throw new JsonException("Unable to convert JSON value to int.");
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        // This method is not needed in your case since you are focused on deserialization.
        writer.WriteStartObject();

        // Serialize properties of YourObjectType here
        writer.WritePropertyName("year"); // Replace with the actual property name
        JsonSerializer.Serialize(writer, value, options);

        // Add more properties as needed

        writer.WriteEndObject();
    }
}


//internal class IntConverter : JsonConverter
//{

//    public override bool CanConvert(Type typeToConvert)
//    {
//        return Int32.TryParse(typeToConvert.ToString(), out _);
//    }

//    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        return Int32.Parse(reader.GetString());
//    }
//}
