using StreamMaster.Domain.Models;

namespace StreamMaster.SchedulesDirect.Domain.Models;

public class VideoStreamConfig : SMChannel
{
    public bool IsDuplicate { get; set; }
    public bool IsDummy { get; set; }
    public int M3UFileId { get; set; }
    public int FilePosition { get; set; }
}