using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace StreamMaster.Ring.API.Entities
{
    public class DoorbotHistoryEvent
    {
        /// <summary>
        /// Unique identifier of this historical event
        /// </summary>
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        /// <summary>
        /// Raw date time string when this event occurred
        /// </summary>
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        /// <summary>
        /// DateTime at which this event occurred 
        /// </summary>
        private DateTime? _createdAtDateTime;
        public DateTime? CreatedAtDateTime
        {
            get
            {
                if (_createdAtDateTime.HasValue) return _createdAtDateTime.Value;

                if (!DateTime.TryParse(CreatedAt, out DateTime result))
                {
                    return null;
                }

                return _createdAtDateTime = result;
            }
        }

        /// <summary>
        /// Boolean indicating if the ring was answered
        /// </summary>
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
