using Reinforced.Typings.Attributes;

using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
[XmlRoot(ElementName = "thumb")]
public class Thumb
{
    [XmlAttribute(AttributeName = "aspect")]
    public string Aspect { get; set; }
    [XmlAttribute(AttributeName = "preview")]
    public string Preview { get; set; }
    [XmlText]
    public string Text { get; set; }
}
