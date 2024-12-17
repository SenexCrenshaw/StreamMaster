using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;

public class SMStream
{
    public static string APIName => "SMStreams";

    [Key]
    public string Id { get; set; } = string.Empty;

    [Column(TypeName = "citext")]
    public string? ClientUserAgent { get; set; }

    [Column(TypeName = "citext")]
    public string? CommandProfileName { get; set; }

    public string? ExtInf { get; set; }

    public int FilePosition { get; set; }
    public bool IsHidden { get; set; } = false;
    public bool IsUserCreated { get; set; } = false;
    public int M3UFileId { get; set; } = 0;
    public int ChannelNumber { get; set; } = 0;

    [Column(TypeName = "citext")]
    public string M3UFileName { get; set; } = string.Empty;

    //public string ShortSMStreamId { get; set; } = UniqueHexGenerator.SMChannelIdEmpty;
    [Column(TypeName = "citext")]
    public string Group { get; set; } = "Dummy";

    [Column(TypeName = "citext")]
    public string EPGID { get; set; } = "Dummy";

    [Column(TypeName = "citext")]
    public string Logo { get; set; } = string.Empty;

    [Column(TypeName = "citext")]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "citext")]
    public string Url { get; set; } = string.Empty;

    [Column(TypeName = "citext")]
    public string StationId { get; set; } = string.Empty;

    public bool IsSystem { get; set; }
    public bool NeedsDelete { get; set; }

    [Column(TypeName = "citext")]
    public string ChannelName { get; set; } = string.Empty;

    [Column(TypeName = "citext")]
    public string TVGName { get; set; } = string.Empty;

    [Column(TypeName = "citext")]
    public string CUID { get; set; } = string.Empty;

    [Column(TypeName = "citext")]
    public string ChannelId { get; set; } = string.Empty;

    public SMStreamTypeEnum SMStreamType { get; set; } = SMStreamTypeEnum.Regular;
}