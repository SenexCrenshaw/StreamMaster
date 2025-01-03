using System.Xml.Serialization;

using Reinforced.Typings.Attributes;

namespace StreamMaster.PlayList.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
[XmlRoot(ElementName = "set")]
public class Set
{
    [XmlElement(ElementName = "name")]
    public string? Name { get; set; }
    [XmlElement(ElementName = "overview")]
    public string? Overview { get; set; }
}
