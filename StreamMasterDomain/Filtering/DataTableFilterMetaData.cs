using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StreamMasterDomain.Filtering;

public class DataTableFilterMetaData
{
    [JsonPropertyName("fieldName")]
    public string FieldName { get; set; }

    [JsonPropertyName("value")]
    public object Value { get; set; }

    [JsonPropertyName("valueType")]
    public string ValueType { get; set; }

    [JsonPropertyName("matchMode")]
    public string MatchMode { get; set; }
}