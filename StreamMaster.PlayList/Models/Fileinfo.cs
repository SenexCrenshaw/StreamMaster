using System.Xml.Serialization;

using Reinforced.Typings.Attributes;

namespace StreamMaster.PlayList.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
[XmlRoot(ElementName = "fileinfo")]
public class Fileinfo
{
    [XmlElement(ElementName = "streamdetails")]
    public Streamdetails? Streamdetails { get; set; }
}
