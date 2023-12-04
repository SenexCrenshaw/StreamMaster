using System.Collections.Generic;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.XmltvXml
{
    public class XmltvRating
    {
        [XmlElement("value")]
        public string Value { get; set; }

        [XmlElement("icon")]
        public List<XmltvIcon> Icons { get; set; }

        [XmlAttribute("system")]
        public string System { get; set; }
    }
}