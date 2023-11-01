using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Team : ITeam
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("isHome")]
    public bool? IsHome { get; set; }

    public Team() { }
}
