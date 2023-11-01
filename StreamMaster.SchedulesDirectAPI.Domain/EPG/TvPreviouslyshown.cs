using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.EPG;

[XmlRoot(ElementName = "previously-shown")]
public class TvPreviouslyshown
{
    [XmlAttribute(AttributeName = "start")]
    public string? Start { get; set; }
}
