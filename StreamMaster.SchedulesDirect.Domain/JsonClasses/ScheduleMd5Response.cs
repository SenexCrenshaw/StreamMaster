using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class ScheduleMd5Response : BaseResponse
    {
        [JsonPropertyName("lastModified")]
        public string LastModified { get; set; } = string.Empty;

        [JsonPropertyName("md5")]
        public string Md5 { get; set; } = string.Empty;
    }
}