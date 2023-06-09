using StreamMasterDomain.Mappings;

namespace StreamMasterDomain.Dto;

public class SettingDto : Setting, IMapFrom<Setting>
{
    public IconFileDto DefaultIconDto { get; set; }

    /// <summary>
    /// The version of the StreamMaster application.
    /// </summary>
    public string Version { get; set; } = "0.2.4";
}
