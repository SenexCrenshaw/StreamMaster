using System.ComponentModel;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.XmltvXml
{
    public class XmltvIcon
    {
        [XmlAttribute("src")]
        public string Src { get; set; }

        [XmlAttribute("width")]
        [DefaultValue(0)]
        public int Width { get; set; }

        [XmlAttribute("height")]
        [DefaultValue(0)]
        public int Height { get; set; }
    }
}