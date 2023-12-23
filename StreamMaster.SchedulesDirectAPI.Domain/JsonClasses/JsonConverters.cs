using System.Text.Json;
using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.JsonClasses
{
    internal class SingleOrListConverter<T> : JsonConverter<List<T>>
    {

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(List<T>);
        }

        public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            string rootString = "";
            try
            {
                using JsonDocument doc = JsonDocument.ParseValue(ref reader);
                JsonElement root = doc.RootElement;
                rootString = root.GetRawText();

                if (typeof(ProgramDescriptions).IsAssignableFrom(typeof(T)))
                {
                    List<T>? item2 = JsonSerializer.Deserialize<List<T>>(root.GetRawText(), options);
                    return JsonSerializer.Deserialize<List<T>>(root.GetRawText(), options);
                }

                if (root.ValueKind == JsonValueKind.Array)
                {
                    List<T>? item2 = JsonSerializer.Deserialize<List<T>>(root.GetRawText(), options);
                    return JsonSerializer.Deserialize<List<T>>(root.GetRawText(), options);
                }
                else
                {
                    T item = JsonSerializer.Deserialize<T>(root.GetRawText(), options);
                    return [item];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {

        }
    }

    //internal class SingleOrArrayConverter<T> : JsonConverter<T[]>
    //{
    //    public override bool CanConvert(Type typeToConvert)
    //    {
    //        return typeToConvert == typeof(T[]);
    //    }

    //    public override T[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    //    {
    //        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
    //        {
    //            JsonElement root = doc.RootElement;
    //            if (root.ValueKind == JsonValueKind.Array)
    //            {
    //                return JsonSerializer.Deserialize<T[]>(root.GetRawText(), options);
    //            }
    //            else
    //            {
    //                T singleValue = JsonSerializer.Deserialize<T>(root.GetRawText(), options);
    //                return new T[] { singleValue };
    //            }
    //        }
    //    }

    //    public override void Write(Utf8JsonWriter writer, T[] value, JsonSerializerOptions options)
    //    {

    //    }
    //}
}
