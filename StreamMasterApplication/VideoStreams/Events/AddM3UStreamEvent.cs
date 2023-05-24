using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Events;

public class AddVideoStreamEvent : BaseEvent
{
    public AddVideoStreamEvent(VideoStreamDto videoStreamsDto)
    {
        VideoStreamsDto = videoStreamsDto;
    }

    public VideoStreamDto VideoStreamsDto { get; }
}
