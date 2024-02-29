using AutoMapper.Configuration.Annotations;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;

public class SMChannel
{
    public StreamingProxyTypes StreamingProxyType { get; set; } = StreamingProxyTypes.SystemDefault;

    [Ignore]
    public ICollection<SMChannelStreamLink> SMStreams { get; set; } = [];

    [Key]
    public int Id { get; set; }

    public bool IsHidden { get; set; } = false;

    public ICollection<StreamGroupSMChannel> StreamGroups { get; set; } = [];

    [Column(TypeName = "citext")]
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
    public string VideoStreamId { get; set; }
}
