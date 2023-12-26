using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.VideoStreams.Events;

public class UpdateVideoStreamEvent(VideoStreamDto videoStream, bool toggelVisibility) : BaseEvent
{
    public bool ToggelVisibility { get; set; } = toggelVisibility;
    public VideoStreamDto VideoStream { get; } = videoStream;
}
