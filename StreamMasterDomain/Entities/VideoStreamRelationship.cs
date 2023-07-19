namespace StreamMasterDomain.Entities;

//public class VideoStreamRelationship
//{
//    public VideoStream ChildVideoStream { get; set; }
//    public int ChildVideoStreamId { get; set; }
//    public VideoStream ParentVideoStream { get; set; }
//    public int ParentVideoStreamId { get; set; }
//    public int Rank { get; set; }
//}

public class VideoStreamsChildVideoStream
{
    public int ChildVideoStreamId { get; set; }
    public int Id { get; set; }
    public int ParentVideoStreamId { get; set; }
    public int Rank { get; set; }
}

public class VideoStreamLink
{
    public int ParentVideoStreamId { get; set; }
    public VideoStream ParentVideoStream { get; set; }

    public int ChildVideoStreamId { get; set; }
    public VideoStream ChildVideoStream { get; set; }

    public int Rank { get; set; } 
}
