using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;

public class Team
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("isHome")]
    public bool? IsHome { get; set; }

    public Team() { }
}
