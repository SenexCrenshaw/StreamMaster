using System.Text.Json.Serialization;

namespace StreamMasterDomain.Filtering;

public class DataTableFilterMetaData
{
    [JsonPropertyName("fieldName")]
    public string FieldName { get; set; }

    [JsonPropertyName("value")]
    public object Value { get; set; }


    [JsonPropertyName("matchMode")]
    public string MatchMode { get; set; }
}