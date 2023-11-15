using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.EPG;

[XmlRoot(ElementName = "video")]
public class TvVideo
{
    [XmlElement(ElementName = "quality")]
    public List<string?> Quality { get; set; }
}
