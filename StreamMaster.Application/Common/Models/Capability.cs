using System.Xml.Serialization;

namespace StreamMaster.Application.Common.Models
{
    [XmlRoot(ElementName = "root")]
    public class Capability
    {
        [XmlElement(ElementName = "device")]
        public Device Device = new Device
        {
            DeviceType = "urn:schemas-upnp-org:device:MediaServer:1",
            FriendlyName = "",
            Manufacturer = "Silicondust",
            ModelName = "HDTC-2US",
            ModelNumber = "HDTC-2US",
            SerialNumber = "device1-18178891",
            UDN = "device1"
        };

        [XmlElement(ElementName = "specVersion")]
        public SpecVersion SpecVersion = new()
        {
            Major = 1,
            Minor = 0,
        };

        [XmlText]
        public string Text = string.Empty;

        [XmlElement(ElementName = "URLBase")]
        public string URLBase = string.Empty;

        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns = "urn:schemas-upnp-org:device-1-0";
        public Capability()
        {

        }
        public Capability(string urlBase, string deviceID)
        {
            URLBase = urlBase;
            Device = new Device
            {
                DeviceType = "urn:schemas-upnp-org:device:MediaServer:1",
                FriendlyName = "",
                Manufacturer = "Silicondust",
                ModelName = "HDTC-2US",
                ModelNumber = "HDTC-2US",
                SerialNumber = deviceID + "18178891",
                UDN = deviceID
            };
        }

    }
}