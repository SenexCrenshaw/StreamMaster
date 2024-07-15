namespace StreamMaster.Application.Common.Events;

public class DeleteChannelsEvent(int channelId) : BaseEvent
{
    public int ChannelId { get; } = channelId;
}
