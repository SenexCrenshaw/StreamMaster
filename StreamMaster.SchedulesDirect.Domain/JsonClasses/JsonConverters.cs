using System.Text.Json;
using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    internal class SingleOrListConverter<T> : JsonConverter<List<T>>
    {
        public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                // Directly inspect the JSON value type
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    return JsonSerializer.Deserialize<List<T>>(ref reader, options) ?? [];
                }
                else
                {
                    // Deserialize single object into a list with one item
                    T? item = JsonSerializer.Deserialize<T>(ref reader, options);
                    return [item];
                }
            }
            catch (Exception ex)
            {
                // You can improve logging with a logging framework if needed
                Console.WriteLine($"Error deserializing: {ex.Message}");
                return [];
            }
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}