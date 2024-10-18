using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class ChimeAlerts
    {
        [JsonPropertyName("connection")]
        public string Connection { get; set; }
    }
}
