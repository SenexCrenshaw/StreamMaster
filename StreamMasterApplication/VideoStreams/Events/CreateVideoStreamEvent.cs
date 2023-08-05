using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Events;

public class CreateVideoStreamEvent : BaseEvent
{
    public CreateVideoStreamEvent(VideoStreamDto videoStreamsDto)
    {
        VideoStreamsDto = videoStreamsDto;
    }

    public VideoStreamDto VideoStreamsDto { get; }
}
