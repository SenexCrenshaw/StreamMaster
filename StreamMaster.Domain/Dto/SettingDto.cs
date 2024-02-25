using StreamMaster.Domain.Configuration;

namespace StreamMaster.Domain.Dto;


public class SettingDto : BaseSettings, IMapFrom<Setting>
{
    public Dictionary<string, FFMPEGProfile> FFMPEGProfiles { get; set; } = [];
    public SDSettings SDSettings { get; set; } = new();
    public HLSSettings HLS { get; set; } = new();

    public string Release { get; set; } = BuildInfo.Release.ToString();

    public string Version { get; set; } = BuildInfo.Version.ToString();

    public string FFMPEGDefaultOptions { get; set; } = BuildInfo.FFMPEGDefaultOptions;

    public bool IsDebug { get; set; } = BuildInfo.IsDebug;
}
