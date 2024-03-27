namespace StreamMaster.Application.VideoStreams.Events;

public class DeleteVideoStreamsEvent(IEnumerable<string> videoStreamIds) : BaseEvent
{
    public IEnumerable<string> VideoStreamIds { get; } = videoStreamIds;
}
