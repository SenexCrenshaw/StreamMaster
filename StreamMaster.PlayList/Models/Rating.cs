using System.Xml.Serialization;

using Reinforced.Typings.Attributes;

namespace StreamMaster.PlayList.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
[XmlRoot(ElementName = "rating")]
public class Rating
{
    [XmlElement(ElementName = "value")]
    public string Value { get; set; } = string.Empty;
    [XmlElement(ElementName = "votes")]
    public string? Votes { get; set; }
    [XmlAttribute(AttributeName = "name")]
    public string? Name { get; set; }
    [XmlAttribute(AttributeName = "max")]
    public string? Max { get; set; }
    [XmlAttribute(AttributeName = "default")]
    public string? Default { get; set; }
}
