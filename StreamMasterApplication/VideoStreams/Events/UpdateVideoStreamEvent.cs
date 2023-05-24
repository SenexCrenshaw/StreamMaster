using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Events;

public class UpdateVideoStreamEvent : BaseEvent
{
    public UpdateVideoStreamEvent(VideoStreamDto videoStreamsDto)
    {
        VideoStreamDto = videoStreamsDto;
    }

    public VideoStreamDto VideoStreamDto { get; }
}
