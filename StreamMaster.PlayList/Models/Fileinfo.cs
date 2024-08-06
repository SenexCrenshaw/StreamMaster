using Reinforced.Typings.Attributes;

using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
[XmlRoot(ElementName = "fileinfo")]
public class Fileinfo
{
    [XmlElement(ElementName = "streamdetails")]
    public Streamdetails Streamdetails { get; set; }
}
