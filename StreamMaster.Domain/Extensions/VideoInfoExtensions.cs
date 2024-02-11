using StreamMaster.Domain.Models;

namespace StreamMaster.Domain.Extensions;

public static class VideoInfoExtensions
{
    public static bool IsValid(this VideoInfo? videoInfo)
    {
        return videoInfo != null && videoInfo.Format != null && videoInfo.Streams != null;
    }
}
