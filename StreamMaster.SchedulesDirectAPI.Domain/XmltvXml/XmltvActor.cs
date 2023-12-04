using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.XmltvXml
{
    public class XmltvActor
    {
        [XmlAttribute("role")]
        public string Role { get; set; }

        [XmlText]
        public string Actor { get; set; }
    }
}