using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class DoNotDisturb
    {
        [JsonPropertyName("seconds_left")]
        public decimal? SecondsLeft { get; set; }
    }
}
