using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class Session
    {
        [JsonPropertyName("profile")]
        public Profile Profile { get; set; }
    }
}