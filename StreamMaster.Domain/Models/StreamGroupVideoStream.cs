using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;

public class StreamGroupVideoStream
{
    public VideoStream ChildVideoStream { get; set; }
    [Column(TypeName = "citext")]
    public string ChildVideoStreamId { get; set; }

    public bool IsReadOnly
    {
        get; set;
    }

    public int StreamGroupId { get; set; }
    public int Rank { get; set; }
}
