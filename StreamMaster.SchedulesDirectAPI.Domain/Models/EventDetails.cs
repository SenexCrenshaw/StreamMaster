using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class EventDetails : IEventDetails
{
    [JsonPropertyName("venue100")]
    public string Venue100 { get; set; }

    [JsonPropertyName("teams")]
    public List<Team> Teams { get; set; }

    [JsonPropertyName("gameDate")]
    public string GameDate { get; set; }

    public EventDetails() { }
}
