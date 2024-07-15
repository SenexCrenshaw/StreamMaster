using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;

public class SetNfo
{
    [XmlElement("name")]
    public string? Name { get; set; }

    [XmlElement("overview")]
    public string? Overview { get; set; }
}
