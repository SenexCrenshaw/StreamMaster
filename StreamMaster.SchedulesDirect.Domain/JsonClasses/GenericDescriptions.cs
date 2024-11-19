using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class GenericDescription : BaseResponse
    {
        [JsonPropertyName("startAirdate")]
        public string StartAirdate { get; set; } = string.Empty;

        [JsonPropertyName("description100")]
        public string? Description100 { get; set; }

        [JsonPropertyName("description1000")]
        public string? Description1000 { get; set; }
    }
}