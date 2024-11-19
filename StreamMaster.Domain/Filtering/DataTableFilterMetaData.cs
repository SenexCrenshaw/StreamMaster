using System.Text.Json.Serialization;

namespace StreamMaster.Domain.Filtering;

public class DataTableFilterMetaData
{
    [JsonPropertyName("fieldName")]
    public string FieldName { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public object Value { get; set; } = string.Empty;


    [JsonPropertyName("matchMode")]
    public string MatchMode { get; set; } = string.Empty;
}