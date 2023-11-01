using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.EPG;

[XmlRoot(ElementName = "icon")]
public class TvIcon
{
    [XmlAttribute(AttributeName = "height")]
    public string? Height { get; set; }

    [XmlAttribute(AttributeName = "src")]
    public string? Src { get; set; }

    [XmlAttribute(AttributeName = "width")]
    public string? Width { get; set; }
}
