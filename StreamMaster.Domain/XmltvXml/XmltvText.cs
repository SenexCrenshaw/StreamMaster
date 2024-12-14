using System.Xml.Serialization;

namespace StreamMaster.Domain.XmltvXml
{
    public class XmltvText
    {
        public XmltvText()
        { }

        public XmltvText(string text)
        {
            Text = text;
        }

        [XmlAttribute("lang")]
        public string? Language { get; set; }

        [XmlText]
        public string? Text { get; set; }
    }
}