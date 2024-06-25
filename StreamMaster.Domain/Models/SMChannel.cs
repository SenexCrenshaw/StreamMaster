using AutoMapper.Configuration.Annotations;

using MessagePack;

using StreamMaster.Domain.Attributes;
using StreamMaster.Domain.Extensions;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace StreamMaster.Domain.Models;

public class SMChannel
{
    [System.ComponentModel.DataAnnotations.Key]
    public int Id { get; set; }

    [Ignore]
    [JsonIgnore]
    [IgnoreMember]
    [IgnoreMap]
    [XmlIgnore]
    public ICollection<SMChannelStreamLink> SMStreams { get; set; } = [];

    [Ignore]
    [JsonIgnore]
    [IgnoreMember]
    [IgnoreMap]
    [XmlIgnore]
    public ICollection<StreamGroupSMChannelLink> StreamGroups { get; set; } = [];
    public static string APIName => "SMChannels";
    public string StreamingProxyType { get; set; } = "SystemDefault";
    public bool IsHidden { get; set; } = false;

    public int ChannelNumber { get; set; } = 0;
    public int TimeShift { get; set; } = 0;

    [Column(TypeName = "citext")]
    public string Group { get; set; } = "All";

    [Column(TypeName = "citext")]
    public string EPGId { get; set; } = string.Empty;

    [Column(TypeName = "citext")]
    public string Logo { get; set; } = string.Empty;

    [Column(TypeName = "citext")]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "citext")]
    public string StationId { get; set; } = string.Empty;

    [Column(TypeName = "citext")]
    public string GroupTitle { get; set; } = string.Empty;

    public VideoStreamHandlers VideoStreamHandler { get; set; } = VideoStreamHandlers.SystemDefault;

    [Column(TypeName = "citext")]
    public string SMChannelId { get; set; } = UniqueHexGenerator.SMChannelIdEmpty;
}
