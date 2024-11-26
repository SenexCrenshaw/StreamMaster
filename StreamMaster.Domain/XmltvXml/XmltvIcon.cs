using System.ComponentModel;
using System.Xml.Serialization;

namespace StreamMaster.Domain.XmltvXml
{
    public class XmltvIcon
    {
        [XmlAttribute("src")]
        public string Src { get; set; } = string.Empty;

        [XmlAttribute("width")]
        [DefaultValue(0)]
        public int Width { get; set; } = 0;

        [XmlAttribute("height")]
        [DefaultValue(0)]
        public int Height { get; set; } = 0;

        public bool ShouldSerializeWidth()
        {
            return Width != 0;
        }

        public bool ShouldSerializeHeight()
        {
            return Height != 0;
        }
    }
}