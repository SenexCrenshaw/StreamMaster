namespace StreamMasterDomain.Entities;

public class VideoStreamLink
{
    public VideoStream ChildVideoStream { get; set; }
    public string ChildVideoStreamId { get; set; }
    public VideoStream ParentVideoStream { get; set; }
    public string ParentVideoStreamId { get; set; }
    public int Rank { get; set; }
}
