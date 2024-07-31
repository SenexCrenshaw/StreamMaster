namespace StreamMaster.Domain.Extensions;

public static class VideoInfoExtensions
{
    public static bool IsValid(this VideoInfo? videoInfo)
    {
        return videoInfo != null && !string.IsNullOrEmpty(videoInfo.StreamName);
    }
}
