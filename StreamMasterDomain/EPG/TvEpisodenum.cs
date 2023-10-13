using System.Xml.Serialization;

namespace StreamMasterDomain.EPG;

[XmlRoot(ElementName = "episode-num")]
public class TvEpisodenum
{
    [XmlAttribute(AttributeName = "system")]
    public string? System { get; set; }

    [XmlText]
    public string? Text { get; set; }
}
