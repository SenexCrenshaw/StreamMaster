namespace StreamMaster.Streams.Domain.Extensions;

public static class VideoOutputProfileExtensions
{
    public static VideoOutputProfileDto ToVideoOutputProfileDto(this VideoOutputProfile videoOutputProfile, string ProfileName)
    {
        return new VideoOutputProfileDto
        {
            Command = videoOutputProfile.Command,
            ProfileName = ProfileName,
            IsReadOnly = videoOutputProfile.IsReadOnly,
            Parameters = videoOutputProfile.Parameters,
            Timeout = videoOutputProfile.Timeout,
            IsM3U8 = videoOutputProfile.IsM3U8
        };
    }
}
