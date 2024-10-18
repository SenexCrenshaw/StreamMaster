using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class HistoryEventRecording
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
