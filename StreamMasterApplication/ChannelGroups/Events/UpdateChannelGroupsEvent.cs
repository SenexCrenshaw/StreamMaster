namespace StreamMasterApplication.ChannelGroups.Events;

public class UpdateChannelGroupsEvent(List<ChannelGroupDto> channelGroups) : BaseEvent
{
    public List<ChannelGroupDto> ChannelGroups { get; } = channelGroups;
}
