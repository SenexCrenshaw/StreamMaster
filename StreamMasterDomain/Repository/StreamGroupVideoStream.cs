namespace StreamMasterDomain.Repository;

public class StreamGroupVideoStream
{
    public VideoStream ChildVideoStream { get; set; }
    public string ChildVideoStreamId { get; set; }

    public bool IsReadOnly
    {
        get; set;
    }

    public int StreamGroupId { get; set; }
    public int Rank { get; set; }
}

public class StreamGroupChannelGroup
{
    public ChannelGroup ChannelGroup { get; set; }
    public int ChannelGroupId { get; set; }
    public StreamGroup StreamGroup { get; set; }
    public int StreamGroupId { get; set; }

   
}
