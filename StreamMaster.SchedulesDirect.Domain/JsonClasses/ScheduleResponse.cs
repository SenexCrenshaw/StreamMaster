using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class ScheduleResponse : BaseResponse
    {
        [JsonPropertyName("stationID")]
        public string StationId { get; set; } = string.Empty;

        [JsonPropertyName("retryTime")]
        public string RetryTime { get; set; } = string.Empty;

        [JsonPropertyName("minDate")]
        public string MinDate { get; set; } = string.Empty;

        [JsonPropertyName("maxDate")]
        public string MaxDate { get; set; } = string.Empty;

        [JsonPropertyName("requestedDate")]
        public string RequestedDate { get; set; } = string.Empty;

        [JsonPropertyName("programs")]
        //[JsonConverter(typeof(SingleOrListConverter<ScheduleProgram>))]
        public List<ScheduleProgram> Programs { get; set; } = [];

        [JsonPropertyName("metadata")]
        public ScheduleMetadata Metadata { get; set; } = new();
    }
}