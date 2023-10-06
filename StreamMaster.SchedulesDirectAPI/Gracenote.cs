using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;

public class Gracenote
{
    [JsonPropertyName("season")]
    public int Season { get; set; }

    [JsonPropertyName("episode")]
    public int Episode { get; set; }

    [JsonPropertyName("totalEpisodes")]
    public int? TotalEpisodes { get; set; }

    [JsonPropertyName("totalSeasons")]
    public int? TotalSeasons { get; set; }

    public Gracenote() { }
}
