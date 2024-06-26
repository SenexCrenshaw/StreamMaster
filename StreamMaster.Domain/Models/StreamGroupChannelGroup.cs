namespace StreamMaster.Domain.Models;

public class StreamGroupChannelGroup
{
    public ChannelGroup ChannelGroup { get; set; }
    public int ChannelGroupId { get; set; }
    public StreamGroup StreamGroup { get; set; }
    public int StreamGroupId { get; set; }

}
