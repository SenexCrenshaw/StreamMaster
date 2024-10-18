using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class Vertex
    {
        [JsonPropertyName("x")]
        public double? X { get; set; }

        [JsonPropertyName("y")]
        public double? Y { get; set; }
    }
}
