using StreamMaster.Domain.Common;

namespace StreamMaster.Application.VideoStreams.Events;

public class DeleteVideoStreamsEvent : BaseEvent
{
    public DeleteVideoStreamsEvent(IEnumerable<string> videoStreamIds)
    {
        VideoStreamIds = videoStreamIds;
    }

    public IEnumerable<string> VideoStreamIds { get; }
}
