using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.XmltvXml
{
    public class XmltvEpisodeNum
    {
        [XmlAttribute("system")]
        public string System { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}