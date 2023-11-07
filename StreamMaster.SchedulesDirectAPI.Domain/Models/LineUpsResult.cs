using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class LineupsResult : ILineupsResult
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("serverID")]
    public string ServerID { get; set; }

    [JsonPropertyName("datetime")]
    public DateTime Datetime { get; set; }

    [JsonPropertyName("lineups")]
    public List<Lineup> Lineups { get; set; } = new();

}
