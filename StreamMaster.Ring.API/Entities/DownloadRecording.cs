using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    /// <summary>
    /// Message providing an URL where to download a recording from
    /// </summary>
    public class DownloadRecording
    {
        /// <summary>
        /// The URL where to download a recording from. If an empty string is returned, it means the Ring service is still preparing the download. Keep making the request until this returns an URL.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}