using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class ProgramTitle
    {
        [JsonPropertyName("title120")]
        public string Title120 { get; set; } = string.Empty;

        [JsonPropertyName("titleLanguage")]
        public string TitleLanguage { get; set; } = string.Empty;
    }
}