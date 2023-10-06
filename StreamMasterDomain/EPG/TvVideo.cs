using System.Xml.Serialization;

namespace StreamMasterDomain.EPG;

[XmlRoot(ElementName = "video")]
public class TvVideo
{
    [XmlElement(ElementName = "quality")]
    public List<string?> Quality { get; set; }
}
