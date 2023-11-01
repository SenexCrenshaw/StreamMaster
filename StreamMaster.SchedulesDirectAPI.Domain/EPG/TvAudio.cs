using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.EPG;

[XmlRoot(ElementName = "audio")]
public class TvAudio
{
    [XmlElement(ElementName = "stereo")]
    public string? Stereo { get; set; }
}
