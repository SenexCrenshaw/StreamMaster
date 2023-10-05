namespace StreamMasterApplication.VideoStreams.Events;

public class UpdateVideoStreamsEvent(IEnumerable<VideoStreamDto> videoStreams) : BaseEvent
{
    public IEnumerable<VideoStreamDto> VideoStreams { get; } = videoStreams;

}
