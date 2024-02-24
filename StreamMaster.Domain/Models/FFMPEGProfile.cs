using StreamMaster.Domain.Attributes;

using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;

[RequireAll]
public class FFMPEGProfile
{
    public int Id { get; set; }

    [Column(TypeName = "citext")]
    public string Name { get; set; }

    [Column(TypeName = "citext")]
    public string Parameters { get; set; }

    public int Timeout { get; set; }

    public bool IsM3U8 { get; set; }

}