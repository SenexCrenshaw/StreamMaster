using System.Xml.Serialization;

namespace StreamMaster.Application.Common.Models
{
    [XmlRoot(ElementName = "device")]
    public class Device
    {
        [XmlElement(ElementName = "deviceType")]
        public string DeviceType = string.Empty;

        [XmlElement(ElementName = "friendlyName")]
        public string FriendlyName = string.Empty;

        [XmlElement(ElementName = "manufacturer")]
        public string Manufacturer = string.Empty;

        [XmlElement(ElementName = "modelName")]
        public string ModelName = string.Empty;

        [XmlElement(ElementName = "modelNumber")]
        public string ModelNumber = string.Empty;

        [XmlElement(ElementName = "serialNumber")]
        public object SerialNumber = string.Empty;

        [XmlElement(ElementName = "UDN")]
        public string UDN = string.Empty;
    }
}