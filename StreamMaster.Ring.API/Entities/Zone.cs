using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class Zone
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("state")]
        public long? State { get; set; }

        [JsonPropertyName("vertex1")]
        public Vertex Vertex1 { get; set; }

        [JsonPropertyName("vertex2")]
        public Vertex Vertex2 { get; set; }

        [JsonPropertyName("vertex3")]
        public Vertex Vertex3 { get; set; }

        [JsonPropertyName("vertex4")]
        public Vertex Vertex4 { get; set; }

        [JsonPropertyName("vertex5")]
        public Vertex Vertex5 { get; set; }

        [JsonPropertyName("vertex6")]
        public Vertex Vertex6 { get; set; }

        [JsonPropertyName("vertex7")]
        public Vertex Vertex7 { get; set; }

        [JsonPropertyName("vertex8")]
        public Vertex Vertex8 { get; set; }
    }
}
