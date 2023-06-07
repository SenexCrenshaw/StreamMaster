namespace StreamMasterDomain.Entities;

public class StreamGroup : BaseEntity
{
    public List<ChannelGroup> ChannelGroups { get; set; } = new();
    public string Name { get; set; } = string.Empty;
    public int StreamGroupNumber { get; set; }
    public List<VideoStream> VideoStreams { get; set; } = new();
}
