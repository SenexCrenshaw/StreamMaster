using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class BaseResponse
    {
        [JsonPropertyName("response")]
        public string Response { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("serverID")]
        public string ServerId { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("datetime")]
        public DateTime Datetime { get; set; }
        public bool ShouldSerializeDatetime() => Datetime.Ticks > 0;

        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }
    }
}