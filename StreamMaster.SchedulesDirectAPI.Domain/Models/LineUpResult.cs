using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;


public class LineupResult
{

    [JsonPropertyName("map")]
    public List<Map> Map { get; set; }

    [JsonPropertyName("stations")]
    public List<Station> Stations { get; set; }

    [JsonPropertyName("metadata")]
    public Metadata Metadata { get; set; }
}
