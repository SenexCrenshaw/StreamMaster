using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class LightSettings
    {
        [JsonPropertyName("brightness")]
        public long? Brightness { get; set; }
    }
}
