namespace StreamMasterApplication.VideoStreams.Events;

public class UpdateVideoStreamsEvent(IEnumerable<VideoStreamDto> videoStreams, bool updateChannelGroup = false) : BaseEvent
{
    public IEnumerable<VideoStreamDto> VideoStreams { get; } = videoStreams;
}
