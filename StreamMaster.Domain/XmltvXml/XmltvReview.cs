using System.Xml.Serialization;

namespace StreamMaster.Domain.XmltvXml
{
    public class XmltvReview
    {
        [XmlAttribute("type")]
        public string? Type { get; set; }

        [XmlAttribute("source")]
        public string? Source { get; set; }

        [XmlAttribute("reviewer")]
        public string? Reviewer { get; set; }

        [XmlAttribute("lang")]
        public string? Lang { get; set; }

        [XmlText]
        public string? Text { get; set; }
    }
}