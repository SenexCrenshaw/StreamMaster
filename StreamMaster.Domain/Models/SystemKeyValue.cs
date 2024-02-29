using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;

public class SystemKeyValue
{
    [Key]
    public int Id { get; set; }
    [Column(TypeName = "citext")]
    public string Key { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string Value { get; set; } = string.Empty;
}
