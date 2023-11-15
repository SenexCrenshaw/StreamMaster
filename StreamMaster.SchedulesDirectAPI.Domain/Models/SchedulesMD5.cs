using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class SchedulesMD5
{
    [JsonPropertyName("stationID")]
    public string StationID { get; set; }

    [JsonPropertyName("programs")]
    public List<MD5Program> Programs { get; set; }

    [JsonPropertyName("metadata")]
    public MD5Metadata Metadata { get; set; }
}
