using System.Xml.Serialization;

using Reinforced.Typings.Attributes;

namespace StreamMaster.PlayList.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
[XmlRoot(ElementName = "uniqueid")]
public class Uniqueid
{
    [XmlAttribute(AttributeName = "type")]
    public string? Type { get; set; }
    [XmlAttribute(AttributeName = "default")]
    public string? Default { get; set; }
    [XmlText]
    public string? Text { get; set; }
}
