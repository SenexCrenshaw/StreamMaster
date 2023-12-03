using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.EPG;

[XmlRoot(ElementName = "tv")]
public class Tv
{
    [XmlElement(ElementName = "channel")]
    public List<TvChannel> Channel { get; set; } = new();

    [XmlAttribute(AttributeName = "guide2go")]
    public string Guide2go { get; set; } = string.Empty;

    [XmlElement(ElementName = "programme")]
    public List<EPGProgramme> Programme { get; set; } = new();

    [XmlAttribute(AttributeName = "source-info-name")]
    public string Sourceinfoname { get; set; } = "Stream Master";

    [XmlAttribute(AttributeName = "generator-info-name")]
    public string Generatorinfoname { get; set; } = "Stream Master";

    [XmlAttribute(AttributeName = "source-info-url")]
    public string Sourceinfourl { get; set; } = "Stream Master";
}
