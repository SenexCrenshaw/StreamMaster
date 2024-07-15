using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;

public class FileInfo
{
    [XmlElement("streamdetails")]
    public StreamDetails? StreamDetails { get; set; }
}
