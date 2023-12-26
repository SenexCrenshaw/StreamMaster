using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.ChannelGroups.Events;

public class DeleteChannelGroupsEvent(IEnumerable<int> channelGroupIds, IEnumerable<VideoStreamDto> videoStreams) : BaseEvent
{
    public IEnumerable<VideoStreamDto> VideoStreams { get; } = videoStreams;
    public IEnumerable<int> ChannelGroupIds { get; } = channelGroupIds;
}
