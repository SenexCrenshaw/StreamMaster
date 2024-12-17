using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses;

public class ScheduleMultipart
{
    [JsonPropertyName("partNumber")]
    public int PartNumber { get; set; }

    [JsonPropertyName("totalParts")]
    public int TotalParts { get; set; }
}
