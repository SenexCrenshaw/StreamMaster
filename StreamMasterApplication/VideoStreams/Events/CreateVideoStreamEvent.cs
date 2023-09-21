namespace StreamMasterApplication.VideoStreams.Events;

public class CreateVideoStreamEvent : BaseEvent
{
    public CreateVideoStreamEvent(VideoStreamDto videoStream)
    {
        VideoStream = videoStream;
    }

    public VideoStreamDto VideoStream { get; }
}
