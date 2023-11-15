using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Lineup : ILineup
{
    [JsonPropertyName("id")]
    public string Id => LineupString;

    [JsonPropertyName("lineup")]
    public string LineupString { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("transport")]
    public string Transport { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; }

    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    [JsonPropertyName("isDeleted")]
    public bool IsDeleted { get; set; }
}