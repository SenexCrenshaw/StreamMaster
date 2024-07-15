using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;

public class SubtitleNfo
{
    [XmlElement("language")]
    public string? Language { get; set; }
}
