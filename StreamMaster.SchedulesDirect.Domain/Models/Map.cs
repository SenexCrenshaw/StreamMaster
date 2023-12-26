using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;

public class Map
{
    [JsonPropertyName("stationID")]
    public string StationId { get; set; }

    [JsonPropertyName("uhfVhf")]
    public int UhfVhf { get; set; }

    [JsonPropertyName("atscMajor")]
    public int AtscMajor { get; set; }

    [JsonPropertyName("atscMinor")]
    public int AtscMinor { get; set; }
}
