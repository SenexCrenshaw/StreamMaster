using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.ChannelGroups.Events;

public class DeleteChannelGroupEvent(int channelGroupId, IEnumerable<VideoStreamDto> videoStreams) : BaseEvent
{
    public IEnumerable<VideoStreamDto> VideoStreams { get; } = videoStreams;
    public int ChannelGroupId { get; } = channelGroupId;
}
