namespace StreamMaster.Domain.Models;

public class StreamGroupChannelGroup
{
    public required ChannelGroup ChannelGroup { get; set; }
    public int ChannelGroupId { get; set; }
    public required StreamGroup StreamGroup { get; set; }
    public int StreamGroupId { get; set; }
}
