using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class AdvancedObjectSettings
    {
        [JsonPropertyName("human_detection_confidence")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public HumanDetectionConfidence HumanDetectionConfidence { get; set; }

        [JsonPropertyName("motion_zone_overlap")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public HumanDetectionConfidence MotionZoneOverlap { get; set; }

        [JsonPropertyName("object_time_overlap")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public HumanDetectionConfidence ObjectTimeOverlap { get; set; }

        [JsonPropertyName("object_size_minimum")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public HumanDetectionConfidence ObjectSizeMinimum { get; set; }

        [JsonPropertyName("object_size_maximum")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public HumanDetectionConfidence ObjectSizeMaximum { get; set; }
    }
}
