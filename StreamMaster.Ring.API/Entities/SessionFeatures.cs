using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class SessionFeatures
    {
        [JsonPropertyName("remote_logging_format_storing")]
        public bool? RemoteLoggingFormatStoring { get; set; }

        [JsonPropertyName("remote_logging_level")]
        public int? RemoteLoggingLevel { get; set; }

        [JsonPropertyName("subscriptions_enabled")]
        public bool? SubscriptionsEnabled { get; set; }

        [JsonPropertyName("stickupcam_setup_enabled")]
        public bool? StickupcamSetupEnabled { get; set; }

        [JsonPropertyName("vod_enabled")]
        public bool? VodEnabled { get; set; }

        [JsonPropertyName("nw_enabled")]
        public bool? NwEnabled { get; set; }

        [JsonPropertyName("nw_user_activated")]
        public bool? NwUserActivated { get; set; }

        [JsonPropertyName("ringplus_enabled")]
        public bool? RingPlusEnabled { get; set; }

        [JsonPropertyName("lpd_enabled")]
        public bool? LdpEnabled { get; set; }

        [JsonPropertyName("reactive_snoozing_enabled")]
        public bool? ReactiveSnoozingEnabled { get; set; }

        [JsonPropertyName("proactive_snoozing_enabled")]
        public bool? ProActiveSnoozingEnabled { get; set; }

        [JsonPropertyName("owner_proactive_snoozing_enabled")]
        public bool? OwnerProActiveSnoozingEnabled { get; set; }

        [JsonPropertyName("live_view_settings_enabled")]
        public bool? LiveviewSettingsEnabled { get; set; }

        [JsonPropertyName("delete_all_settings_enabled")]
        public bool? DeleteAllSettingsEnabled { get; set; }

        [JsonPropertyName("power_cable_enabled")]
        public bool? PowerCableEnabled { get; set; }

        [JsonPropertyName("device_health_alerts_enabled")]
        public bool? DeviceHealthAlertsEnabled { get; set; }

        [JsonPropertyName("chime_pro_enabled")]
        public bool? ChimeProEnabled { get; set; }

        [JsonPropertyName("multiple_calls_enabled")]
        public bool? MultipleCallsEnabled { get; set; }

        [JsonPropertyName("ujet_enabled")]
        public bool? UjetEnabled { get; set; }

        [JsonPropertyName("multiple_delete_enabled")]
        public bool? MultipleDeleteEnabled { get; set; }

        [JsonPropertyName("delete_all_enabled")]
        public bool? DeleteAllEnabled { get; set; }

        [JsonPropertyName("lpd_motion_announcement_enabled")]
        public bool? LdpMotionAnnouncementEnabled { get; set; }

        [JsonPropertyName("starred_events_enabled")]
        public bool? StarredEventsEnabled { get; set; }

        [JsonPropertyName("chime_dnd_enabled")]
        public bool? ChimeDndEnabled { get; set; }

        [JsonPropertyName("video_search_enabled")]
        public bool? VideoSearchEnabled { get; set; }

        [JsonPropertyName("floodlight_cam_enabled")]
        public bool? FlooglightCamEnabled { get; set; }

        [JsonPropertyName("nw_larger_area_enabled")]
        public bool? NwLargerAreaEnabled { get; set; }

        [JsonPropertyName("ring_cam_battery_enabled")]
        public bool? RingCamBatteryEnabled { get; set; }

        [JsonPropertyName("elite_cam_enabled")]
        public bool? EliteCamEnabled { get; set; }

        [JsonPropertyName("doorbell_v2_enabled")]
        public bool? DoorbellV2Enabled { get; set; }

        [JsonPropertyName("spotlight_battery_dashboard_controls_enabled")]
        public bool? SpotlightBatteryDashboardControlsEnabled { get; set; }

        [JsonPropertyName("bypass_account_verification")]
        public bool? ByPassAccountVerification { get; set; }

        [JsonPropertyName("legacy_cvr_retention_enabled")]
        public bool? LegacyCvrRetentionEnabled { get; set; }

        [JsonPropertyName("new_dashboard_enabled")]
        public bool? NewDashboardEnabled { get; set; }

        [JsonPropertyName("ring_cam_enabled")]
        public bool? RingCamEnabled { get; set; }
    }
}
