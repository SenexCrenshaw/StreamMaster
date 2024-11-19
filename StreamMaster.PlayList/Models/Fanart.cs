using System.Xml.Serialization;

using Reinforced.Typings.Attributes;

namespace StreamMaster.PlayList.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
[XmlRoot(ElementName = "fanart")]
public class Fanart
{
    [XmlElement(ElementName = "thumb")]
    public Thumb? Thumb { get; set; }
}
