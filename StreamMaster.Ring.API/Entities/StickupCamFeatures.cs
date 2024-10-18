using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class StickupCamFeatures
    {
        [JsonPropertyName("motions_enabled")]
        public bool? MotionsEnabled { get; set; }

        [JsonPropertyName("show_recordings")]
        public bool? ShowRecordings { get; set; }

        [JsonPropertyName("show_vod_settings")]
        public bool? ShowVodSettings { get; set; }
    }
}
