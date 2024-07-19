using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.Models;

namespace StreamMaster.SchedulesDirect.Domain.Models;

public class VideoStreamConfig : SMChannel
{
    public bool IsDuplicate { get; set; }
    public bool IsDummy { get; set; }
    public bool IsCustom => M3UFileId == EPGHelper.CustomPlayListId;
    public int FilePosition { get; set; }
}