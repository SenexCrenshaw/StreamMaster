using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.VideoStreams.Events;

public class CreateVideoStreamEvent : BaseEvent
{
    public CreateVideoStreamEvent(VideoStreamDto videoStream)
    {
        VideoStream = videoStream;
    }

    public VideoStreamDto VideoStream { get; }
}
