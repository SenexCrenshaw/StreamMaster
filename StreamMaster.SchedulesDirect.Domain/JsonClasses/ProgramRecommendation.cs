using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class ProgramRecommendation
    {
        [JsonPropertyName("programID")]
        public string ProgramId { get; set; } = string.Empty;

        [JsonPropertyName("title120")]
        public string Title120 { get; set; } = string.Empty;
    }
}