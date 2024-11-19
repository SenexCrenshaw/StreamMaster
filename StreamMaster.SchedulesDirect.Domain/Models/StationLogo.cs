using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;

public class StationLogo : Logo
{
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }
}
