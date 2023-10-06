using System.Xml.Serialization;

namespace StreamMasterDomain.EPG;

[XmlRoot(ElementName = "sub-title")]
public class TvSubtitle
{
    [XmlAttribute(AttributeName = "lang")]
    public string? Lang { get; set; }

    [XmlText]
    public string? Text { get; set; }
}
