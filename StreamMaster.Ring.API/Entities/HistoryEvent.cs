using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace StreamMaster.Ring.API.Entities
{
    public class HistoryEvent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("answered")]
        public bool Answered { get; set; }

        [JsonPropertyName("events")]
        public List<object> Events { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("favorite")]
        public bool Favorite { get; set; }

        [JsonPropertyName("snapshot_url")]
        public string SnapshotUrl { get; set; }

        [JsonPropertyName("recording")]
        public DoorbotHistoryEventRecording Recording { get; set; }

        [JsonPropertyName("doorbot")]
        public Doorbot Doorbot { get; set; }
    }
}
