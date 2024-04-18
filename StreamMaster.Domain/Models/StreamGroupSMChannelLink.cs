using AutoMapper.Configuration.Annotations;

using MessagePack;

using Reinforced.Typings.Attributes;

using System.Text.Json.Serialization;
namespace StreamMaster.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true)]
public class StreamGroupSMChannelLink
{
    [Ignore]
    [JsonIgnore]
    [IgnoreMember]
    public SMChannel SMChannel { get; set; }
    public int SMChannelId { get; set; }

    [Ignore]
    [JsonIgnore]
    [IgnoreMember]
    public StreamGroup StreamGroup { get; set; }
    public int StreamGroupId { get; set; }


    public bool IsReadOnly { get; set; }
    public int Rank { get; set; }
}

