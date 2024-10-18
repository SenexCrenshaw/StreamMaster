using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    /// <summary>
    /// Message providing an URL where to download a shared recording from
    /// </summary>
    public class SharedRecording
    {
        /// <summary>
        /// Not sure what it would contain. Returned an empty string during my tests.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// The URL where to download a shared recording from
        /// </summary>
        [JsonPropertyName("wrapper_url")]
        public string WrapperUrl { get; set; }
    }
}