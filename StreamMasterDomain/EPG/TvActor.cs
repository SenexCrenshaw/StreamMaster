using System.Xml.Serialization;

namespace StreamMasterDomain.EPG;

[XmlRoot(ElementName = "actor")]
public class TvActor
{
    [XmlAttribute(AttributeName = "role")]
    public string? Role { get; set; }

    [XmlText]
    public string? Text { get; set; }
}
