using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.XmltvXml
{
    public class XmltvPreviouslyShown
    {
        [XmlAttribute("start")]
        public string Start { get; set; }

        [XmlAttribute("channel")]
        public string Channel { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}