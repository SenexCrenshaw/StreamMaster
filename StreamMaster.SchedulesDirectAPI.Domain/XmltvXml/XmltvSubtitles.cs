using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.XmltvXml
{
    public class XmltvSubtitles
    {
        [XmlElement("language")]
        public string Language { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }
    }
}