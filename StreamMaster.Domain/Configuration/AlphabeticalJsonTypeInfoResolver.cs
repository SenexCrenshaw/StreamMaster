using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
namespace StreamMaster.Domain.Configuration;

public class AlphabeticalJsonTypeInfoResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo? typeInfo = base.GetTypeInfo(type, options);

        if (typeInfo?.Kind == JsonTypeInfoKind.Object)
        {
            List<JsonPropertyInfo> sortedProperties = [.. typeInfo.Properties.OrderBy(p => p.Name, StringComparer.Ordinal)];

            typeInfo.Properties.Clear();
            foreach (JsonPropertyInfo? property in sortedProperties)
            {
                typeInfo.Properties.Add(property);
            }
        }

        return typeInfo;
    }
}