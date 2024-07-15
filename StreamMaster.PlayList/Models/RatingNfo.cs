using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;

public class RatingNfo
{
    [XmlAttribute("name")]
    public string? Name { get; set; }

    [XmlAttribute("max")]
    public int Max { get; set; }

    [XmlAttribute("default")]
    public bool Default { get; set; }

    [XmlElement("value")]
    public float Value { get; set; }

    [XmlElement("votes")]
    public int Votes { get; set; }
}
