using AutoMapper.Configuration.Annotations;

using MessagePack;

using StreamMaster.Domain.Attributes;

using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace StreamMaster.Domain.Dto;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
//[MessagePackObject(keyAsPropertyName: true)]
public class SMChannelDto : SMChannel, IMapFrom<SMChannel>
{
    [Ignore, JsonIgnore, IgnoreMember, IgnoreMap, XmlIgnore, TsIgnore]
    public new ICollection<SMChannelStreamLink> SMStreams { get; set; } = [];

    [Ignore, JsonIgnore, IgnoreMember, IgnoreMap, XmlIgnore, TsIgnore]
    public new ICollection<SMChannelChannelLink> SMChannels { get; set; } = [];

    [Ignore, JsonIgnore, IgnoreMember, IgnoreMap, XmlIgnore, TsIgnore]
    public new ICollection<StreamGroupSMChannelLink> StreamGroups { get; set; } = [];

    public List<SMStreamDto> SMStreamDtos { get; set; } = [];
    public List<SMChannelDto> SMChannelDtos { get; set; } = [];
    //public List<StreamGroupDto> StreamGroupDtos { get; set; } = [];
    public List<int> StreamGroupIds { get; set; } = [];
    public string StreamUrl { get; set; } = string.Empty;
    public int CurrentRank { get; set; } = -1;
    public int Rank { get; set; }
}
