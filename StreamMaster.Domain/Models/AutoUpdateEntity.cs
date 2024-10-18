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
    public int HoursToUpdate { get; set; }
    [Column(TypeName = "citext")]
    public string? Url { get; set; }
}
