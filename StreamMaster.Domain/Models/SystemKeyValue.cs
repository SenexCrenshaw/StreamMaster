using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;

public class SystemKeyValue
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    [Column(TypeName = "citext")]
    public string Key { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string Value { get; set; } = string.Empty;
}
