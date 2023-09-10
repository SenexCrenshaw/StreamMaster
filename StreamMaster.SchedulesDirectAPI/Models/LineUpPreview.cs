using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Models;

public class LineUpPreview
{
    public int Id { get; set; }
    [JsonPropertyName("affiliate")]
    public string Affiliate { get; set; }

    [JsonPropertyName("callsign")]
    public string Callsign { get; set; }

    [JsonPropertyName("channel")]
    public string Channel { get; set; }

    public string LineUp { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
