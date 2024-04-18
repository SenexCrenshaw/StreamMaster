using AutoMapper.Configuration.Annotations;

using MessagePack;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StreamMaster.Domain.Models;

public class StreamGroup : BaseEntity
{
    public StreamGroup()
    {
        ChildVideoStreams = [];
        ChannelGroups = [];
        SMChannels = [];
    }

    public string FFMPEGProfileId { get; set; } = string.Empty;
    public ICollection<StreamGroupChannelGroup> ChannelGroups { get; set; }
    public ICollection<StreamGroupVideoStream> ChildVideoStreams { get; set; }
    [Ignore]
    [JsonIgnore]
    [IgnoreMember]
    public ICollection<StreamGroupSMChannelLink> SMChannels { get; set; }
    public bool IsReadOnly { get; set; } = false;
    public bool AutoSetChannelNumbers { get; set; } = false;
    [Column(TypeName = "citext")]
    public string Name { get; set; } = string.Empty;

}
