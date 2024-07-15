using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;

public class StreamDetails
{
    [XmlElement("video")]
    public VideoNfo? Video { get; set; }

    [XmlElement("audio")]
    public List<AudioNfo>? Audio { get; set; }

    [XmlElement("subtitle")]
    public List<SubtitleNfo>? Subtitle { get; set; }
}
