using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;

public class VideoNfo
{
    [XmlElement("codec")]
    public string? Codec { get; set; }

    [XmlElement("aspect")]
    public float Aspect { get; set; }

    [XmlElement("width")]
    public int Width { get; set; }

    [XmlElement("height")]
    public int Height { get; set; }

    [XmlElement("durationinseconds")]
    public int DurationInSeconds { get; set; }
}
