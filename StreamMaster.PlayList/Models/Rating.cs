using Reinforced.Typings.Attributes;

using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
[XmlRoot(ElementName = "rating")]
public class Rating
{
    [XmlElement(ElementName = "value")]
    public string Value { get; set; }
    [XmlElement(ElementName = "votes")]
    public string Votes { get; set; }
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }
    [XmlAttribute(AttributeName = "max")]
    public string Max { get; set; }
    [XmlAttribute(AttributeName = "default")]
    public string Default { get; set; }
}
