namespace StreamMasterDomain.Entities;

public class VideoStreamLink
{
    public VideoStream ChildVideoStream { get; set; }
    public int ChildVideoStreamId { get; set; }
    public VideoStream ParentVideoStream { get; set; }
    public int ParentVideoStreamId { get; set; }
    public int Rank { get; set; }
}
