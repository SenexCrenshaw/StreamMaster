using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Events;

public class UpdateVideoStreamsEvent : BaseEvent
{
    public UpdateVideoStreamsEvent(IEnumerable<VideoStreamDto> videoStreamDtos)
    {
        VideoStreamDtos = videoStreamDtos;
    }

    public IEnumerable<VideoStreamDto> VideoStreamDtos { get; }
}
