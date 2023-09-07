namespace StreamMasterApplication.VideoStreams.Events;

public class UpdateVideoStreamsEvent(IEnumerable<VideoStreamDto> videoStreams, bool updateChannelGroup = false) : BaseEvent
{
    public bool UpdateChannelGroup { get; } = updateChannelGroup;
    public IEnumerable<VideoStreamDto> VideoStreams { get; } = videoStreams;
}
