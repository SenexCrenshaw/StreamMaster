using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class ScheduleMetadata : IScheduleMetadata
{
    [JsonPropertyName("modified")]
    public DateTime Modified { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }

    [JsonPropertyName("startDate")]
    public string StartDate { get; set; }
}
