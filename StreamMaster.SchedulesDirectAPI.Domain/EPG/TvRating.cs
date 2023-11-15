using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.EPG;

[XmlRoot(ElementName = "rating")]
public class TvRating
{
    [XmlAttribute(AttributeName = "system")]
    public string? System { get; set; }

    [XmlElement(ElementName = "value")]
    public string? Value { get; set; }
}
