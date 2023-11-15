using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Gracenote : IGracenote
{
    [JsonPropertyName("season")]
    public int Season { get; set; }

    [JsonPropertyName("episode")]
    public int Episode { get; set; }

    [JsonPropertyName("totalEpisodes")]
    public int? TotalEpisodes { get; set; }

    [JsonPropertyName("totalSeasons")]
    public int? TotalSeasons { get; set; }

}
