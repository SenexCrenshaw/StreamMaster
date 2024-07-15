using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;

public class UniqueIdNfo
{
    [XmlAttribute("type")]
    public string? Type { get; set; }

    [XmlAttribute("default")]
    public bool Default { get; set; }

    [XmlText]
    public string? Value { get; set; }
}
