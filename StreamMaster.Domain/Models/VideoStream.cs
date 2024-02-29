using AutoMapper.Configuration.Annotations;

using StreamMaster.Domain.Extensions;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;

public class VideoStream
{
    public VideoStream()
    {
        StreamGroups = [];
    }

    public StreamingProxyTypes StreamingProxyType { get; set; } = StreamingProxyTypes.SystemDefault;

    [Ignore]
    public ICollection<VideoStreamLink> ChildVideoStreams { get; set; }

    public int FilePosition { get; set; }

    [Key]
    [Column(TypeName = "citext")]
    public string Id { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    public bool IsHidden { get; set; } = false;

    public bool IsReadOnly { get; set; } = false;

    public bool IsUserCreated { get; set; } = false;

    public int M3UFileId { get; set; } = 0;

    public ICollection<StreamGroupVideoStream> StreamGroups { get; set; }

    public StreamingProxyTypes StreamProxyType { get; set; } = StreamingProxyTypes.SystemDefault;
    [Column(TypeName = "citext")]
    public string M3UFileName { get; set; }
    public int Tvg_chno { get; set; } = 0;

    [Column(TypeName = "citext")]
    public string ShortId { get; set; } = UniqueHexGenerator.ShortIdEmpty;

    public int TimeShift { get; set; } = 0;

    [Column(TypeName = "citext")]
    public string Tvg_group { get; set; } = "All";

    [Column(TypeName = "citext")]
    public string Tvg_ID { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string Tvg_logo { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string Tvg_name { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string StationId { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string Url { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string GroupTitle { get; set; } = string.Empty;

    public int User_Tvg_chno { get; set; } = 0;
    [Column(TypeName = "citext")]
    public string User_Tvg_group { get; set; } = "All";
    [Column(TypeName = "citext")]
    public string User_Tvg_ID { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string User_Tvg_logo { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string User_Tvg_name { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string User_Url { get; set; } = string.Empty;

    public VideoStreamHandlers VideoStreamHandler { get; set; } = VideoStreamHandlers.SystemDefault;
}
