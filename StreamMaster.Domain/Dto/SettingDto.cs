using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Configuration;

using System.Xml.Serialization;

namespace StreamMaster.Domain.Dto;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class SettingDto : BaseSettings, IMapFrom<Setting>
{
    [XmlIgnore]
    public SDSettings SDSettings { get; set; } = new();
    [XmlIgnore]
    public HLSSettings HLS { get; set; } = new();

    public string Release { get; set; } = BuildInfo.Release.ToString();

    public string Version { get; set; } = BuildInfo.Version.ToString();

    public string FFMPEGDefaultOptions { get; set; } = BuildInfo.FFMPEGDefaultOptions;

    public bool IsDebug { get; set; } = BuildInfo.IsDebug;
}
