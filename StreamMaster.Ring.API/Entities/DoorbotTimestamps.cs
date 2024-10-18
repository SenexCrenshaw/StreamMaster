using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace StreamMaster.Ring.API.Entities
{
    /// <summary>
    /// Timestamps related to a specific doorbot
    /// </summary>
    public class DoorbotTimestamps
    {
        /// <summary>
        /// Collection of doorbot timestamps
        /// </summary>
        [JsonPropertyName("timestamps")]
        public List<DoorbotTimestamp> Timestamp { get; set; }
    }
}
