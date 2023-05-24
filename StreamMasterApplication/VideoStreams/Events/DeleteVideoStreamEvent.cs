namespace StreamMasterApplication.VideoStreams.Events;

public class DeleteVideoStreamEvent : BaseEvent
{
    public DeleteVideoStreamEvent(int videoFileId)
    {
        VideoFileId = videoFileId;
    }

    public int VideoFileId { get; }
}
