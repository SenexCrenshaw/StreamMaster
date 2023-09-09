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
