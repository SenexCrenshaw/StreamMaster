namespace StreamMasterApplication.Common.Events;

public class DeleteChannelGroupEvent : BaseEvent
{
    public DeleteChannelGroupEvent(int channelGroupId)
    {
        ChannelGroupId = channelGroupId;
    }

    public int ChannelGroupId { get; }
}
