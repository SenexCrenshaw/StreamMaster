namespace StreamMasterApplication.ChannelGroups.Events;

public class UpdateChannelGroupEvent(ChannelGroupDto channelGroup, bool toggelVisibility) : BaseEvent
{
    public bool ToggelVisibility { get; set; } = toggelVisibility;
    public ChannelGroupDto ChannelGroup { get; } = channelGroup;
}
