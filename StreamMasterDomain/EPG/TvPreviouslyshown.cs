using System.Xml.Serialization;

namespace StreamMasterDomain.EPG;

[XmlRoot(ElementName = "previously-shown")]
public class TvPreviouslyshown
{
    [XmlAttribute(AttributeName = "start")]
    public string? Start { get; set; }
}
