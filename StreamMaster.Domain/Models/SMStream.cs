using StreamMaster.Domain.Extensions;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;

public class SMStream
{

    public static string APIName => "SMStreams";
    [Key]
    [Column(TypeName = "citext")]
    public string Id { get; set; } = string.Empty;
    public int FilePosition { get; set; }
    public bool IsHidden { get; set; } = false;
    public bool IsUserCreated { get; set; } = false;
    public int M3UFileId { get; set; } = 0;
    public int ChannelNumber { get; set; } = 0;

    [Column(TypeName = "citext")]
    public string M3UFileName { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string ShortId { get; set; } = UniqueHexGenerator.ShortIdEmpty;
    [Column(TypeName = "citext")]
    public string Group { get; set; } = "All";
    [Column(TypeName = "citext")]
    public string EPGID { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string Logo { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string Name { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string Url { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string StationId { get; set; } = string.Empty;
}
