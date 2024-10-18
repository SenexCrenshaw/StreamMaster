using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class DoorbotAlerts
    {
        [JsonPropertyName("connection")]
        public string Connection { get; set; }
    }
}
