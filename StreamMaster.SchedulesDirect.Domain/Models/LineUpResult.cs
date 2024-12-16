using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class LineupResult
{
    [JsonPropertyName("map")]
    public List<Map>? Map { get; set; }

    [JsonPropertyName("stations")]
    public List<Station> Stations { get; set; } = [];

    [JsonPropertyName("metadata")]
    public Metadata? Metadata { get; set; }
}