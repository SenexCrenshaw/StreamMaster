using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.XmltvXml
{
    public class XmltvAudio
    {
        [XmlElement("present")]
        public string Present { get; set; }

        [XmlElement("stereo")]
        public string Stereo { get; set; }
    }
}