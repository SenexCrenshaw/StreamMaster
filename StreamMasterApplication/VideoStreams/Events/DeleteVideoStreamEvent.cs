namespace StreamMasterApplication.VideoStreams.Events;

public class DeleteVideoStreamEvent : BaseEvent
{
    public DeleteVideoStreamEvent(string videoFileId)
    {
        VideoFileId = videoFileId;
    }

    public string VideoFileId { get; }
}

public class DeleteVideoStreamsEvent : BaseEvent
{
    public DeleteVideoStreamsEvent(List<string> videoFileIds)
    {
        VideoFileIds = videoFileIds;
    }

    public List<string> VideoFileIds { get; }
}
