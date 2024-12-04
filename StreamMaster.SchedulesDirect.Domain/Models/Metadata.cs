using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class Metadata
{
    [JsonPropertyName("lineup")]
    public string? Lineup { get; set; }

    [JsonPropertyName("modified")]
    public DateTime? Modified { get; set; }

    [JsonPropertyName("transport")]
    public string? Transport { get; set; }

    [JsonPropertyName("modulation")]
    public string Modulation { get; set; } = string.Empty;
}
