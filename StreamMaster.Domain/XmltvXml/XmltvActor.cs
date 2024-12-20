using System.Xml.Serialization;

namespace StreamMaster.Domain.XmltvXml
{
    public class XmltvActor
    {
        [XmlAttribute("role")]
        public string? Role { get; set; }

        [XmlText]
        public string Actor { get; set; } = string.Empty;
    }
}