using System.Collections.Generic;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.XmltvXml
{
    [XmlRoot("tv")]
    public class XMLTV
    {
        [XmlAttribute("date")]
        public string Date { get; set; }

        [XmlAttribute("source-info-url")]
        public string SourceInfoUrl { get; set; }

        [XmlAttribute("source-info-name")]
        public string SourceInfoName { get; set; }

        [XmlAttribute("source-data-url")]
        public string SourceDataUrl { get; set; }

        [XmlAttribute("generator-info-name")]
        public string GeneratorInfoName { get; set; }

        [XmlAttribute("generator-info-url")]
        public string GeneratorInfoUrl { get; set; }

        [XmlElement("channel")]
        public List<XmltvChannel> Channels { get; set; } = new List<XmltvChannel>();

        [XmlElement("programme")]
        public List<XmltvProgramme> Programs { get; set; } = new List<XmltvProgramme>();
    }
}