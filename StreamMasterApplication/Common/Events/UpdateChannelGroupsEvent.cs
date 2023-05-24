using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Events;

public class UpdateChannelGroupsEvent : BaseEvent
{
    public UpdateChannelGroupsEvent(IEnumerable<ChannelGroupDto> channelGroups)
    {
        ChannelGroups = channelGroups;
    }

    public IEnumerable<ChannelGroupDto> ChannelGroups { get; }
}
