using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class Chime
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; }

        [JsonPropertyName("time_zone")]
        public string TimeZone { get; set; }

        [JsonPropertyName("firmware_version")]
        public string FirmwareVersion { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("settings")]
        public ChimeSettings Settings { get; set; }

        [JsonPropertyName("features")]
        public ChimeFeatures Features { get; set; }

        [JsonPropertyName("owned")]
        public bool Owned { get; set; }

        [JsonPropertyName("alerts")]
        public ChimeAlerts Alerts { get; set; }

        [JsonPropertyName("do_not_disturb")]
        public DoNotDisturb DoNotDisturb { get; set; }

        [JsonPropertyName("stolen")]
        public bool Stolen { get; set; }

        [JsonPropertyName("owner")]
        public Owner Owner { get; set; }
    }
}
