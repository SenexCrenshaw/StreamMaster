using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.ChannelGroups.Events;

public class CreateChannelGroupEvent(ChannelGroupDto channelGroup) : BaseEvent
{
    public ChannelGroupDto ChannelGroup { get; } = channelGroup;
}
