using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class UserStatus : BaseResponse
    {
        [JsonPropertyName("account")]
        public StatusAccount Account { get; set; } = new();

        [JsonPropertyName("lineups")]
        // //[JsonConverter(typeof(SingleOrListConverter<StatusLineup>))]
        public List<StatusLineup> Lineups { get; set; } = [];

        [JsonPropertyName("lastDataUpdate")]
        public DateTime LastDataUpdate { get; set; } = DateTime.MinValue;

        [JsonPropertyName("notifications")]
        //[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Notifications { get; set; } = [];

        [JsonPropertyName("systemStatus")]
        //[JsonConverter(typeof(SingleOrListConverter<SystemStatus>))]
        public List<SystemStatus> SystemStatus { get; set; } = [];
    }
}