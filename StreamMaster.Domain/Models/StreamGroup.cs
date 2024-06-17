using AutoMapper.Configuration.Annotations;

using MessagePack;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StreamMaster.Domain.Models;

public class StreamGroup : BaseEntity
{
    public static string APIName => "StreamGroups";


    //public string FFMPEGProfileId { get; set; } = string.Empty;

    public List<string> StreamGroupProfiles { get; set; } = [];

    public ICollection<StreamGroupChannelGroup> ChannelGroups { get; set; } = [];


    [Ignore]
    [JsonIgnore]
    [IgnoreMember]
    public ICollection<StreamGroupSMChannelLink> SMChannels { get; set; } = [];

    public bool IsReadOnly { get; set; } = false;
    public bool AutoSetChannelNumbers { get; set; } = false;
    [Column(TypeName = "citext")]
    public string Name { get; set; } = string.Empty;

}
public class StreamGroupProfile
{
    public FileOutputProfile FileOutputProfile { get; set; }
    public VideoOutputProfile VideoOutputProfile { get; set; }
}