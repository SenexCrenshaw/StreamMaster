namespace StreamMaster.Application.VideoStreams.Events;

public class DeleteVideoStreamEvent(string videoStreamId, ChannelGroup? ChannelGroup) : BaseEvent
{
    public ChannelGroup? ChannelGroup { get; } = ChannelGroup;
    public string VideoStreamId { get; } = videoStreamId;
}
