using System.Xml.Serialization;

namespace StreamMasterDomain.EPG;

[XmlRoot(ElementName = "tv")]
public class Tv
{
    [XmlElement(ElementName = "channel")]
    public List<TvChannel> Channel { get; set; } = new();

    [XmlAttribute(AttributeName = "guide2go")]
    public string Guide2go { get; set; } = string.Empty;

    [XmlElement(ElementName = "programme")]
    public List<Programme> Programme { get; set; } = new();

    [XmlAttribute(AttributeName = "source-info-name")]
    public string Sourceinfoname { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "generator-info-name")]
    public string Generatorinfoname { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "source-info-url")]
    public string Sourceinfourl { get; set; } = string.Empty;
}
