using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.XmltvXml
{
    public class XmltvEpisodeNum
    {
        [XmlAttribute("system")]
        public string System { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}