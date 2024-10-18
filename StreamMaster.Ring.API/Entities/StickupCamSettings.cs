using StreamMaster.Ring.API.Converters;

using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class StickupCamSettings
    {
        [JsonPropertyName("enable_vod")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonConverter(typeof(BooleanConverter))]
        public bool? EnableVod { get; set; }

        [JsonPropertyName("exposure_control")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? ExposureControl { get; set; }

        [JsonPropertyName("motion_snooze_preset_profile")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string MotionSnoozePresetProfile { get; set; }

        [JsonPropertyName("motion_snooze_presets")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[] MotionSnoozePresets { get; set; }

        [JsonPropertyName("live_view_preset_profile")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string LiveViewPresetProfile { get; set; }

        [JsonPropertyName("live_view_presets")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[] LiveViewPresets { get; set; }

        [JsonPropertyName("pir_sensitivity_1")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? PirSensitivity1 { get; set; }

        [JsonPropertyName("vod_suspended")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? VodSuspended { get; set; }

        [JsonPropertyName("doorbell_volume")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? DoorbellVolume { get; set; }

        [JsonPropertyName("vod_status")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string VodStatus { get; set; }

        [JsonPropertyName("advanced_motion_detection_enabled")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? AdvancedMotionDetectionEnabled { get; set; }

        [JsonPropertyName("advanced_motion_zones")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AdvancedMotionZones AdvancedMotionZones { get; set; }

        [JsonPropertyName("advanced_motion_detection_human_only_mode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? AdvancedMotionDetectionHumanOnlyMode { get; set; }

        [JsonPropertyName("enable_audio_recording")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? EnableAudioRecording { get; set; }

        [JsonPropertyName("light_settings")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LightSettings LightSettings { get; set; }

        [JsonPropertyName("enable_white_leds")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? EnableWhiteLeds { get; set; }
    }
}
