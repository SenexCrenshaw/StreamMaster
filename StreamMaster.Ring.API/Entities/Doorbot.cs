using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class Doorbot
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
