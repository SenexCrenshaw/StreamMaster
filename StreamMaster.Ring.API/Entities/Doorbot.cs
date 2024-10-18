using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class Doorbot
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; }

        [JsonPropertyName("time_zone")]
        public string TimeZone { get; set; }

        [JsonPropertyName("subscribed")]
        public bool? Subscribed { get; set; }

        [JsonPropertyName("subscribed_motions")]
        public bool? SubscribedMotions { get; set; }

        [JsonPropertyName("battery_life")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BatteryLife { get; set; }

        [JsonPropertyName("external_connection")]
        public bool? ExternalConnection { get; set; }

        [JsonPropertyName("firmware_version")]
        public string FirmwareVersion { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("features")]
        public DoorbotFeatures Features { get; set; }

        [JsonPropertyName("owned")]
        public bool? Owned { get; set; }

        [JsonPropertyName("alerts")]
        public DoorbotAlerts Alerts { get; set; }

        [JsonPropertyName("motion_snooze")]
        public object MotionSnooze { get; set; }

        [JsonPropertyName("stolen")]
        public bool? Stolen { get; set; }

        [JsonPropertyName("owner")]
        public Owner Owner { get; set; }
    }
}
