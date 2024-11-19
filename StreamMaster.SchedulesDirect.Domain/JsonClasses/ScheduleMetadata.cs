using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses;

public class ScheduleMetadata
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("modified")]
    public string Modified { get; set; } = string.Empty;

    [JsonPropertyName("md5")]
    public string Md5 { get; set; } = string.Empty;

    [JsonPropertyName("startDate")]
    public string StartDate { get; set; } = string.Empty;
}
