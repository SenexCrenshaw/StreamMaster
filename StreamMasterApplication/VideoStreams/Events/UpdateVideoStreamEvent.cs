namespace StreamMasterApplication.VideoStreams.Events;

public class UpdateVideoStreamEvent(VideoStreamDto videoStream, bool updateChannelGroup) : BaseEvent
{
    public bool UpdateChannelGroup { get; set; } = updateChannelGroup;
    public VideoStreamDto VideoStream { get; } = videoStream;
}
