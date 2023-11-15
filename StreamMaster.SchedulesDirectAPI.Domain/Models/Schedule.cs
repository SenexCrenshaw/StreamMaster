using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Schedule : ISchedule
{
    [JsonPropertyName("stationID")]
    public string StationID { get; set; }

    [JsonPropertyName("programs")]
    public List<Program> Programs { get; set; }

    [JsonPropertyName("metadata")]
    public ScheduleMetadata Metadata { get; set; }
}
