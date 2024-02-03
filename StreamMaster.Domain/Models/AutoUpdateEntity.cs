using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;
public class AutoUpdateEntity : CacheEntity
{
    public AutoUpdateEntity()
    {
        HoursToUpdate = 72;
        AutoUpdate = true;
    }

    public bool AutoUpdate { get; set; }
    [Column(TypeName = "citext")]
    public string Description { get; set; } = string.Empty;
    public int HoursToUpdate { get; set; }
    [Column(TypeName = "citext")]
    public string? Url { get; set; }
}
