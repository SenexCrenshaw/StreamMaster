using Reinforced.Typings.Attributes;

using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
[XmlRoot(ElementName = "audio")]
public class Audio
{
    [XmlElement(ElementName = "bitrate")]
    public string Bitrate { get; set; }
    [XmlElement(ElementName = "channels")]
    public string Channels { get; set; }
    [XmlElement(ElementName = "codec")]
    public string Codec { get; set; }
    [XmlElement(ElementName = "language")]
    public string Language { get; set; }
}
