using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;

public class AudioNfo
{
    [XmlElement("codec")]
    public string? Codec { get; set; }

    [XmlElement("language")]
    public string? Language { get; set; }

    [XmlElement("channels")]
    public int Channels { get; set; }
}
