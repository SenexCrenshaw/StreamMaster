using AutoMapper.Configuration.Annotations;

using MessagePack;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StreamMaster.Domain.Models;

public class StreamGroup : BaseEntity
{
    public static string APIName => "StreamGroups";

    public List<StreamGroupChannelGroup> ChannelGroups { get; set; } = [];
    public List<StreamGroupProfile> StreamGroupProfiles { get; set; } = [];


    [Ignore]
    [JsonIgnore]
    [IgnoreMember]
    public ICollection<StreamGroupSMChannelLink> SMChannels { get; set; } = [];

    public bool IsReadOnly { get; set; } = false;
    public bool IgnoreExistingChannelNumbers { get; set; } = true;
    public bool AutoSetChannelNumbers { get; set; } = true;
    public int StartingChannelNumber { get; set; } = 1;

    [Column(TypeName = "citext")]
    public string Name { get; set; } = string.Empty;

}
