using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;

public class EventDetails
{
    [JsonPropertyName("venue100")]
    public string Venue100 { get; set; }

    [JsonPropertyName("teams")]
    public List<Team> Teams { get; set; }

    [JsonPropertyName("gameDate")]
    public string GameDate { get; set; }

    public EventDetails() { }
}
