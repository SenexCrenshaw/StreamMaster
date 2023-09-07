namespace StreamMasterApplication.VideoStreams.Events;

public class DeleteVideoStreamsEvent : BaseEvent
{
    public DeleteVideoStreamsEvent(IEnumerable<string> videoStreamIds)
    {
        VideoStreamIds = videoStreamIds;
    }

    public IEnumerable<string> VideoStreamIds { get; }
}
