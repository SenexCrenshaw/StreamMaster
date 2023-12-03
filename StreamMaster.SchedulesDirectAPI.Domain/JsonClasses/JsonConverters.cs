using System;
using System.Collections.Generic;
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
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                JsonElement root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Array)
                {
                    return JsonSerializer.Deserialize<List<T>>(root.GetRawText(), options);
                }
                else
                {
                    T item = JsonSerializer.Deserialize<T>(root.GetRawText(), options);
                    return new List<T> { item };
                }
            }
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

    internal class SingleOrArrayConverter<T> : JsonConverter<T[]>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(T[]);
        }

        public override T[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                JsonElement root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Array)
                {
                    return JsonSerializer.Deserialize<T[]>(root.GetRawText(), options);
                }
                else
                {
                    T item = JsonSerializer.Deserialize<T>(root.GetRawText(), options);
                    return new T[] { item };
                }
            }
        }

        public override void Write(Utf8JsonWriter writer, T[] value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
