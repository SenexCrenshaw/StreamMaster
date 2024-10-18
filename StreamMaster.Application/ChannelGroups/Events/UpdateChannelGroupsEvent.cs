namespace StreamMaster.Application.ChannelGroups.Events;

public class UpdateChannelGroupsEvent(List<ChannelGroup> channelGroups) : BaseEvent
{
    public List<ChannelGroup> ChannelGroups { get; } = channelGroups;
}
