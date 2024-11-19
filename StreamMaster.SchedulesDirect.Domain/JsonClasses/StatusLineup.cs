using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class StatusLineup
    {
        [JsonPropertyName("lineup")]
        public string Lineup { get; set; } = string.Empty;

        [JsonPropertyName("modified")]
        public string Modified { get; set; } = string.Empty;

        [JsonPropertyName("uri")]
        public string Uri { get; set; } = string.Empty;

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}