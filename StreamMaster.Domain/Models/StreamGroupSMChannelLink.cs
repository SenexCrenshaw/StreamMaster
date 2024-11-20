using System.Text.Json.Serialization;

using AutoMapper.Configuration.Annotations;

using MessagePack;
namespace StreamMaster.Domain.Models;
#pragma warning disable CS8618
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
#pragma warning restore CS8618 
