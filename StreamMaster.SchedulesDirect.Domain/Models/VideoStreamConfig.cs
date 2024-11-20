using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.Models;

namespace StreamMaster.SchedulesDirect.Domain.Models;

public class VideoStreamConfig : SMChannel
{
    public bool IsDuplicate { get; set; }
    public bool IsDummy { get; set; }
    public bool IsCustom => M3UFileId == EPGHelper.CustomPlayListId;
    public bool IsIntro => M3UFileId == EPGHelper.IntroPlayListId;
    public int FilePosition { get; set; }
    public CommandProfileDto CommandProfile { get; set; } = new();
    public OutputProfileDto OutputProfile { get; set; } = new();
}