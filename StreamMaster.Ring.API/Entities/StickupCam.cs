using StreamMaster.Ring.API.Converters;

using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class StickupCam
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? Id { get; set; }

        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Description { get; set; }

        [JsonPropertyName("device_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string DeviceId { get; set; }

        [JsonPropertyName("time_zone")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string TimeZone { get; set; }

        [JsonPropertyName("subscribed")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Subscribed { get; set; }

        [JsonPropertyName("subscribed_motions")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? SubscribedMotions { get; set; }

        [JsonPropertyName("battery_life")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonConverter(typeof(BatteryLifeConverter))]
        public int? BatteryLife { get; set; }

        [JsonPropertyName("external_connection")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ExternalConnection { get; set; }

        [JsonPropertyName("firmware_version")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string FirmwareVersion { get; set; }

        [JsonPropertyName("kind")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Kind { get; set; }

        [JsonPropertyName("latitude")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Longitude { get; set; }

        [JsonPropertyName("address")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Address { get; set; }

        [JsonPropertyName("settings")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public StickupCamSettings Settings { get; set; }

        [JsonPropertyName("features")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public StickupCamFeatures Features { get; set; }

        [JsonPropertyName("owned")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Owned { get; set; }

        [JsonPropertyName("alerts")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public StickupCamAlerts Alerts { get; set; }

        [JsonPropertyName("motion_snooze")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string MotionSnooze { get; set; }

        [JsonPropertyName("stolen")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Stolen { get; set; }

        [JsonPropertyName("location_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? LocationId { get; set; }

        [JsonPropertyName("ring_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string RingId { get; set; }

        [JsonPropertyName("owner")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Owner Owner { get; set; }

        [JsonPropertyName("battery_life_2")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? BatteryLife2 { get; set; }

        [JsonPropertyName("battery_voltage")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? BatteryVoltage { get; set; }

        [JsonPropertyName("battery_voltage_2")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? BatteryVoltage2 { get; set; }

        [JsonPropertyName("led_status")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string LedStatus { get; set; }

        [JsonPropertyName("siren_status")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DeviceStatus SirenStatus { get; set; }

        [JsonPropertyName("night_mode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? NightMode { get; set; }
    }
}