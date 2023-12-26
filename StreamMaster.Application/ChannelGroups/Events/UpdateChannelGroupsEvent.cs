using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.ChannelGroups.Events;

public class UpdateChannelGroupsEvent(List<ChannelGroupDto> channelGroups) : BaseEvent
{
    public List<ChannelGroupDto> ChannelGroups { get; } = channelGroups;
}
