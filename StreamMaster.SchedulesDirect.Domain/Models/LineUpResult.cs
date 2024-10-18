using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;

public class LineupResult
{
    [JsonPropertyName("map")]
    public required List<Map> Map { get; set; }

    [JsonPropertyName("stations")]
    public required List<Station> Stations { get; set; }

    [JsonPropertyName("metadata")]
    public Metadata? Metadata { get; set; }
}
