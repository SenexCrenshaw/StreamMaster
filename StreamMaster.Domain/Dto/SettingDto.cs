using StreamMaster.Domain.Common;
using StreamMaster.Domain.Mappings;

namespace StreamMaster.Domain.Dto;


public class SettingDto : BaseSettings, IMapFrom<BaseSettings>
{

    public string Release { get; set; } = BuildInfo.Release.ToString();

    public string Version { get; set; } = BuildInfo.Version.ToString();

    public string FFMPEGDefaultOptions { get; set; } = BuildInfo.FFMPEGDefaultOptions;

    public bool IsDebug { get; set; } = BuildInfo.IsDebug;
}
