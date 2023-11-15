using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class StationId(string stationID)
{
    [JsonPropertyName("stationID")]
    public string StationID { get; set; } = stationID;
}
