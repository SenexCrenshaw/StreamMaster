using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class ScheduleRequest
    {
        [JsonPropertyName("stationID")]
        public string StationId { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        //[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] Date { get; set; } = [];
    }
}