using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;

public class ActorNfo
{
    [XmlElement("name")]
    public string? Name { get; set; }

    [XmlElement("role")]
    public string? Role { get; set; }

    [XmlElement("order")]
    public int Order { get; set; }

    [XmlElement("thumb")]
    public string? Thumb { get; set; }
}
