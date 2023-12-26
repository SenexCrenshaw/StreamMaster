using StreamMaster.Domain.Common;

namespace StreamMaster.Application.Common.Models
{
    public class Discover
    {
        public Discover()
        { }

        public Discover(string urlBase, int deviceID, int tunerCount)
        {
            string version = BuildInfo.Version.ToString();

            BaseURL = urlBase;
            DeviceAuth = "StreamMaster";
            DeviceID = $"2022-6MOKBM:{deviceID}";
            FirmwareName = $"bin_{version}";
            FirmwareVersion = version;
            FriendlyName = "StreamMaster";
            LineupURL = $"{urlBase}/lineup.json";
            Manufacturer = "stream master";
            ModelNumber = version;
            TunerCount = tunerCount;
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
