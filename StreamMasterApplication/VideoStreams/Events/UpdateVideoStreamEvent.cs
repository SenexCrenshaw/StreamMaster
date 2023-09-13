namespace StreamMasterApplication.VideoStreams.Events;

public class UpdateVideoStreamEvent(VideoStreamDto videoStream, bool updateChannelGroup, bool toggelVisibility) : BaseEvent
{
    public bool ToggelVisibility { get; set; } = toggelVisibility;
    public bool UpdateChannelGroup { get; set; } = updateChannelGroup;
    public VideoStreamDto VideoStream { get; } = videoStream;
}
