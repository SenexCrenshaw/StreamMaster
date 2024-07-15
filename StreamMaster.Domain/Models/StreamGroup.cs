using AutoMapper.Configuration.Annotations;

using MessagePack;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StreamMaster.Domain.Models;

public class StreamGroup : BaseEntity
{
    public static string APIName => "StreamGroups";
    public string DeviceID { get; set; } = string.Empty;
    public List<StreamGroupChannelGroup> ChannelGroups { get; set; } = [];
    public List<StreamGroupProfile> StreamGroupProfiles { get; set; } = [];


    [Ignore]
    [JsonIgnore]
    [IgnoreMember]
    public ICollection<StreamGroupSMChannelLink> SMChannels { get; set; } = [];

    public bool IsReadOnly { get; set; } = false;

    public bool IsSystem { get; set; } = false;

    [Column(TypeName = "citext")]
    public string Name { get; set; } = string.Empty;

}
