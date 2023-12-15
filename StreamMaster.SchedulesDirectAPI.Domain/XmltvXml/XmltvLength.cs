using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.XmltvXml
{
    public class XmltvLength
    {
        [XmlAttribute("units")]
        public string Units { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}