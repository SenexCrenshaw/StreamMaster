using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class MD5Metadata
{
    [JsonPropertyName("modified")]
    public DateTime Modified { get; set; }

    [JsonPropertyName("modifiedEpoch")]
    public int ModifiedEpoch { get; set; }

    [JsonPropertyName("hash")]
    public string Hash { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }

    [JsonPropertyName("startDate")]
    public string StartDate { get; set; }
}
