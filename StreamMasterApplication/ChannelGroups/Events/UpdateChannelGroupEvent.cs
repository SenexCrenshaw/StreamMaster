namespace StreamMasterApplication.ChannelGroups.Events;

public class UpdateChannelGroupEvent(ChannelGroupDto channelGroup) : BaseEvent
{
    public ChannelGroupDto ChannelGroup { get; } = channelGroup;
}
