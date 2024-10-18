using Reinforced.Typings.Attributes;

using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
[XmlRoot(ElementName = "video")]
public class Video
{
    [XmlElement(ElementName = "aspect")]
    public string Aspect { get; set; }
    [XmlElement(ElementName = "bitrate")]
    public string Bitrate { get; set; }
    [XmlElement(ElementName = "codec")]
    public string Codec { get; set; }
    [XmlElement(ElementName = "framerate")]
    public string Framerate { get; set; }
    [XmlElement(ElementName = "height")]
    public string Height { get; set; }
    [XmlElement(ElementName = "scantype")]
    public string Scantype { get; set; }
    [XmlElement(ElementName = "width")]
    public string Width { get; set; }
    [XmlElement(ElementName = "duration")]
    public string Duration { get; set; }
    [XmlElement(ElementName = "durationinseconds")]
    public string Durationinseconds { get; set; }
}
