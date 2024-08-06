using Reinforced.Typings.Attributes;

using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
[XmlRoot(ElementName = "streamdetails")]
public class Streamdetails
{
    [XmlElement(ElementName = "video")]
    public Video Video { get; set; }
    [XmlElement(ElementName = "audio")]
    public Audio Audio { get; set; }
    [XmlElement(ElementName = "subtitle")]
    public Subtitle Subtitle { get; set; }
}
