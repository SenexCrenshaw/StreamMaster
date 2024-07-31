using AutoMapper.Configuration.Annotations;

using MessagePack;

using StreamMaster.Domain.Attributes;

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

    //[Column(TypeName = "citext")]
    //public string CommandProfileName { get; set; } = BuildInfo.DefaultCommandProfileName;

    public bool IsHidden { get; set; } = false;
    public bool IsCustomStream { get; set; } = false;

    public string StreamID { get; set; } = string.Empty;

    public int M3UFileId { get; set; }
    public int ChannelNumber { get; set; } = 0;
    public int TimeShift { get; set; } = 0;

    [Column(TypeName = "citext")]
    public string Group { get; set; } = "Dummy";

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

    //public VideoStreamHandlers VideoStreamHandler { get; set; } = VideoStreamHandlers.SystemDefault;

    //[Column(TypeName = "citext")]
    //public string ShortSMChannelId { get; set; } = UniqueHexGenerator.SMChannelIdEmpty;
}
