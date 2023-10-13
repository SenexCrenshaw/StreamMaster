using System.Xml.Serialization;

namespace StreamMasterDomain.EPG;

[XmlRoot(ElementName = "audio")]
public class TvAudio
{
    [XmlElement(ElementName = "stereo")]
    public string? Stereo { get; set; }
}
