using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Multipart
{
    [JsonPropertyName("partNumber")]
    public int PartNumber { get; set; }

    [JsonPropertyName("totalParts")]
    public int TotalParts { get; set; }
}
