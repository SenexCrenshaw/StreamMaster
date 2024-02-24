using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;

public class StreamGroup : BaseEntity
{
    public StreamGroup()
    {
        ChildVideoStreams = [];
        ChannelGroups = [];
    }

    public string FFMPEGProfileId { get; set; }
    public ICollection<StreamGroupChannelGroup> ChannelGroups { get; set; }
    public ICollection<StreamGroupVideoStream> ChildVideoStreams { get; set; }
    public bool IsReadOnly { get; set; } = false;
    public bool AutoSetChannelNumbers { get; set; } = false;
    [Column(TypeName = "citext")]
    public string Name { get; set; } = string.Empty;
    //public int StreamGroupNumber { get; set; }
}
