using System.Xml.Serialization;

namespace StreamMasterApplication.Common.Models
{
    [XmlRoot(ElementName = "specVersion")]
    public class SpecVersion
    {
        [XmlElement(ElementName = "major")]
        public int Major;

        [XmlElement(ElementName = "minor")]
        public int Minor;
    }
}