using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

using AutoMapper.Configuration.Annotations;

using MessagePack;

namespace StreamMaster.Domain.Models;

public class StreamGroupBase : BaseEntity
{
    public string DeviceID { get; set; } = string.Empty;

    [Ignore]
    [JsonIgnore]
    [IgnoreMember]
    [IgnoreMap]
    [XmlIgnore]
    public ICollection<StreamGroupSMChannelLink> SMChannels { get; set; } = [];

    public bool IsReadOnly { get; set; } = false;
    public int ShowIntros { get; set; } = 0; // 0: None 1: First Time 2: Always
    public bool IsSystem { get; set; } = false;

    [Column(TypeName = "citext")]
    public string Name { get; set; } = string.Empty;

    public string GroupKey { get; set; } = string.Empty;
    public bool CreateSTRM { get; set; }
}

public class StreamGroup : StreamGroupBase
{
    public static string APIName => "StreamGroups";
    public List<StreamGroupChannelGroup> ChannelGroups { get; set; } = [];
    public List<StreamGroupProfile> StreamGroupProfiles { get; set; } = [];
}