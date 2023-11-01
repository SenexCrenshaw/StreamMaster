using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Map : IMap
{
    [JsonPropertyName("stationID")]
    public string StationID { get; set; }

    [JsonPropertyName("uhfVhf")]
    public int UhfVhf { get; set; }

    [JsonPropertyName("atscMajor")]
    public int AtscMajor { get; set; }

    [JsonPropertyName("atscMinor")]
    public int AtscMinor { get; set; }
}
