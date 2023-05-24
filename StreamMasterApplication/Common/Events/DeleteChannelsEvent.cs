namespace StreamMasterApplication.Common.Events;

public class DeleteChannelsEvent : BaseEvent
{
    public DeleteChannelsEvent(int channelId)
    {
        ChannelId = channelId;
    }

    public int ChannelId { get; }
}
