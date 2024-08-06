using Reinforced.Typings.Attributes;

using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
[XmlRoot(ElementName = "ratings")]
public class Ratings
{
    [XmlElement(ElementName = "rating")]
    public List<Rating> Rating { get; set; }
}
