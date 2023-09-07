namespace StreamMasterApplication.ChannelGroups.Events;

public class DeleteChannelGroupsEvent(IEnumerable<int> channelGroupIds, IEnumerable<VideoStreamDto> videoStreams) : BaseEvent
{
    public IEnumerable<VideoStreamDto> VideoStreams { get; } = videoStreams;
    public IEnumerable<int> ChannelGroupIds { get; } = channelGroupIds;
}
