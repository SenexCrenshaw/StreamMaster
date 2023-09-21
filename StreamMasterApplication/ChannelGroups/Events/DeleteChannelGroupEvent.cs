namespace StreamMasterApplication.ChannelGroups.Events;

public class DeleteChannelGroupEvent(int channelGroupId, IEnumerable<VideoStreamDto> videoStreams) : BaseEvent
{
    public IEnumerable<VideoStreamDto> VideoStreams { get; } = videoStreams;
    public int ChannelGroupId { get; } = channelGroupId;
}
