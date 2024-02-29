using StreamMaster.Domain.Dto;

namespace StreamMaster.Domain.Common;


public class UpdateSettingResponse
{
    public bool NeedsLogOut { get; set; }
    public SettingDto Settings { get; set; }
}
