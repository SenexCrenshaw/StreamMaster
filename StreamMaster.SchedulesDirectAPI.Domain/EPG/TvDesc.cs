using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.EPG;

[XmlRoot(ElementName = "desc")]
public class TvDesc
{
    [XmlAttribute(AttributeName = "lang")]
    public string? Lang { get; set; }

    [XmlText]
    public string? Text { get; set; }
}
