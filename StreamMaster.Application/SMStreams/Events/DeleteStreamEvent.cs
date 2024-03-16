namespace StreamMaster.Application.SMStreams.Events;

public class DeleteStreamEvent(string videoStreamId, ChannelGroup? ChannelGroup) : BaseEvent
{
    public ChannelGroup? ChannelGroup { get; } = ChannelGroup;
    public string VideoStreamId { get; } = videoStreamId;
}
