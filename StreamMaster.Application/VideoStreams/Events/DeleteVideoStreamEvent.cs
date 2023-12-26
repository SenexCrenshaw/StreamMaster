using StreamMaster.Domain.Common;

namespace StreamMaster.Application.VideoStreams.Events;

public class DeleteVideoStreamEvent : BaseEvent
{
    public DeleteVideoStreamEvent(string videoStreamId)
    {
        VideoStreamId = videoStreamId;
    }

    public string VideoStreamId { get; }
}
