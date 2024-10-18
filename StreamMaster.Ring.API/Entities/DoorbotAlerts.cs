using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class StickupCamAlerts
    {
        [JsonPropertyName("connection")]
        public string Connection { get; set; }
    }
}
