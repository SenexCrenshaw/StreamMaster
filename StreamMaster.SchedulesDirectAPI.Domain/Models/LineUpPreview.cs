using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class LineupPreview : ILineupPreview
{
    public int Id { get; set; }
    [JsonPropertyName("affiliate")]
    public string Affiliate { get; set; }

    [JsonPropertyName("callsign")]
    public string Callsign { get; set; }

    [JsonPropertyName("channel")]
    public string Channel { get; set; }

    public string Lineup { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
