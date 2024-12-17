using Reinforced.Typings.Attributes;

using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
[XmlRoot(ElementName = "subtitle")]
public class Subtitle
{
    [XmlElement(ElementName = "language")]
    public string? Language { get; set; }
}
