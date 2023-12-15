using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Metadata 
{
    [JsonPropertyName("lineup")]
    public string Lineup { get; set; }

    [JsonPropertyName("modified")]
    public DateTime Modified { get; set; }

    [JsonPropertyName("transport")]
    public string Transport { get; set; }
}
