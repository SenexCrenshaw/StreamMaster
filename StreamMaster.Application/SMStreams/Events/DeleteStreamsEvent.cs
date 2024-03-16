namespace StreamMaster.Application.SMStreams.Events;

public class DeleteStreamsEvent(IEnumerable<string> videoStreamIds) : BaseEvent
{
    public IEnumerable<string> VideoStreamIds { get; } = videoStreamIds;
}
