using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.EPG;

[XmlRoot(ElementName = "credits")]
public class TvCredits
{
    [XmlElement(ElementName = "actor")]
    public List<TvActor>? Actor { get; set; }

    [XmlElement(ElementName = "director")]
    public List<string>? Director { get; set; }

    [XmlElement(ElementName = "presenter")]
    public List<string>? Presenter { get; set; }

    [XmlElement(ElementName = "producer")]
    public List<string>? Producer { get; set; }

    [XmlElement(ElementName = "writer")]
    public List<string>? Writer { get; set; }
}
