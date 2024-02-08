using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;

public class VideoStreamLink
{
    public VideoStream ChildVideoStream { get; set; }
    [Column(TypeName = "citext")]
    public string ChildVideoStreamId { get; set; }
    public VideoStream ParentVideoStream { get; set; }
    [Column(TypeName = "citext")]
    public string ParentVideoStreamId { get; set; }
    public int Rank { get; set; }
}
