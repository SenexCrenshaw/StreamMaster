using System.Text.Json.Serialization;
using System;

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

        [JsonPropertyName("kind")]
        public string Kind { get; set; }
    }
}
