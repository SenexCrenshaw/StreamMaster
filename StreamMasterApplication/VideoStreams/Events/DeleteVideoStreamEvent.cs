namespace StreamMasterApplication.VideoStreams.Events;

public class DeleteVideoStreamEvent : BaseEvent
{
    public DeleteVideoStreamEvent(int videoFileId)
    {
        VideoFileId = videoFileId;
    }

    public int VideoFileId { get; }
}

public class DeleteVideoStreamsEvent : BaseEvent
{
    public DeleteVideoStreamsEvent(List<int> videoFileIds)
    {
        VideoFileIds = videoFileIds;
    }

    public List<int> VideoFileIds { get; }
}