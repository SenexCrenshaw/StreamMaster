namespace StreamMasterDomain.Entities;

public class StreamGroupVideoStream
{
    public int Id { get; set; }
    public int Rank { get; set; }
    public int StreamGroupId { get; set; }
    public int VideoStreamId { get; set; }
}

public class StreamGroupChannelGroup
{
    public int ChannelGroupId { get; set; }
    public int Id { get; set; }
    public int StreamGroupId { get; set; }
}
