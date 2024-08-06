using Reinforced.Typings.Attributes;

using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
[XmlRoot(ElementName = "actor")]
public class Actor
{
    [XmlElement(ElementName = "name")]
    public string Name { get; set; }
    [XmlElement(ElementName = "role")]
    public string Role { get; set; }
    [XmlElement(ElementName = "order")]
    public string Order { get; set; }
    [XmlElement(ElementName = "thumb")]
    public string Thumb { get; set; }
}
