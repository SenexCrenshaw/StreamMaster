using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class ChimeSettings
    {
        [JsonPropertyName("volume")]
        public int Volume { get; set; }

        [JsonPropertyName("ding_audio_user_id")]
        public string DingAudioUserId { get; set; }

        [JsonPropertyName("ding_audio_id")]
        public string DingAudioId { get; set; }

        [JsonPropertyName("motion_audio_user_id")]
        public string MotionAudioUserId { get; set; }

        [JsonPropertyName("motion_audio_id")]
        public string MotionAudioId { get; set; }
    }
}
