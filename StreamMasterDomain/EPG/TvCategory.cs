using System.Xml.Serialization;

namespace StreamMasterDomain.EPG;

[XmlRoot(ElementName = "category")]
public class TvCategory
{
    [XmlAttribute(AttributeName = "lang")]
    public string? Lang { get; set; }

    [XmlText]
    public string? Text { get; set; }
}
