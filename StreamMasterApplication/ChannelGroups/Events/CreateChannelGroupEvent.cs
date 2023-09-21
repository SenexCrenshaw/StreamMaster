namespace StreamMasterApplication.ChannelGroups.Events;

public class CreateChannelGroupEvent(ChannelGroupDto channelGroup) : BaseEvent
{
    public ChannelGroupDto ChannelGroup { get; } = channelGroup;
}
