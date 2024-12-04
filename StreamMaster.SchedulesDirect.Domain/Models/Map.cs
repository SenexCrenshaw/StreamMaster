using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class Map
{
    [JsonPropertyName("channel")]
    public string Channel { get; set; } = string.Empty;

    [JsonPropertyName("stationID")]
    public string? StationId { get; set; }

    [JsonPropertyName("uhfVhf")]
    public int? UhfVhf { get; set; }

    [JsonPropertyName("atscMajor")]
    public int? AtscMajor { get; set; }

    [JsonPropertyName("atscMinor")]
    public int? AtscMinor { get; set; }
}
