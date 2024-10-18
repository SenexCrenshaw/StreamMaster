using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class DoorbotHistoryEventRecording
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
