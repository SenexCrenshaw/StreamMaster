using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.VideoStreams.Events;

public class UpdateVideoStreamsEvent(IEnumerable<VideoStreamDto> videoStreams) : BaseEvent
{
    public IEnumerable<VideoStreamDto> VideoStreams { get; } = videoStreams;

}
