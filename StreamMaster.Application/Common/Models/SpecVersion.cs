using System.Xml.Serialization;

namespace StreamMaster.Application.Common.Models
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