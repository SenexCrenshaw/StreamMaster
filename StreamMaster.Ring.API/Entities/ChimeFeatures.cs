using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class ChimeFeatures
    {
        [JsonPropertyName("ringtones_enabled")]
        public bool RingtonesEnabled { get; set; }
    }
}
