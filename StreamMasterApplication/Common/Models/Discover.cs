using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Models
{
    public class Discover
    {
        public Discover()
        { }

        public Discover(SettingDto setting, string urlBase, int deviceID)
        {
            BaseURL = urlBase;
            DeviceAuth = setting.AppName;
            DeviceID = $"2022-6MOKBM:{deviceID}";
            FirmwareName = $"bin_{setting.Version}";
            FirmwareVersion = setting.Version;
            FriendlyName = setting.AppName;
            LineupURL = $"{urlBase}/lineup.json";
            Manufacturer = "stream master";
            ModelNumber = setting.Version;
            TunerCount = 2;
        }

        public string BaseURL { get; set; } = string.Empty;
        public string DeviceAuth { get; set; } = string.Empty;
        public string DeviceID { get; set; } = "device1";
        public string FirmwareName { get; set; } = string.Empty;
        public string FirmwareVersion { get; set; } = string.Empty;
        public string FriendlyName { get; set; } = string.Empty;
        public string LineupURL { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string ModelNumber { get; set; } = string.Empty;
        public int TunerCount { get; set; }
    }
}
