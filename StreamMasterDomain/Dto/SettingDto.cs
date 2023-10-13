using StreamMasterDomain.Attributes;
using StreamMasterDomain.Mappings;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class SettingDto : BaseSettings, IMapFrom<BaseSettings>
{

    public string Release { get; set; } = BuildInfo.Release.ToString();

    public string Version { get; set; } = BuildInfo.Version.ToString();

    public string FFMPEGDefaultOptions { get; set; } = BuildInfo.FFMPEGDefaultOptions;

    public bool IsDebug { get; set; } = BuildInfo.IsDebug;
}
