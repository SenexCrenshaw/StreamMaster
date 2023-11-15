using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.EPG;

[XmlRoot(ElementName = "actor")]
public class TvActor
{
    [XmlAttribute(AttributeName = "role")]
    public string? Role { get; set; }

    [XmlText]
    public string? Text { get; set; }
}
