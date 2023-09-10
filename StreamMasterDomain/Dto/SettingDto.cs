using StreamMasterDomain.Mappings;

namespace StreamMasterDomain.Dto;

public class SettingDto : Setting, IMapFrom<Setting>
{
    public string Release { get; set; } = BuildInfo.Release.ToString();
    public string Version { get; set; } = BuildInfo.Version.ToString();
    public string FFMPEGDefaultOptions { get; set; } = BuildInfo.FFMPEGDefaultOptions;
}
