using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.EPG;

[XmlRoot(ElementName = "channel")]
public class TvChannel
{
    [XmlElement(ElementName = "display-name")]
    public List<string>? Displayname { get; set; }

    [XmlElement(ElementName = "icon")]
    public TvIcon? Icon { get; set; }

    [XmlAttribute(AttributeName = "id")]
    public string? Id { get; set; }
}
