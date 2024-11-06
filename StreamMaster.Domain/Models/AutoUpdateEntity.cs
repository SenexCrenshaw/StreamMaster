using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;
public class AutoUpdateEntity : CacheEntity
{
    public AutoUpdateEntity()
    {
        HoursToUpdate = 72;
        AutoUpdate = true;
    }

    public DateTime LastWrite()
    {
        string fileName = Path.Combine(DirectoryLocation, Source);
        DateTime ret = File.Exists(fileName)
            ? File.GetLastWriteTimeUtc(fileName)
            : File.Exists(fileName + ".gz")
            ? File.GetLastWriteTimeUtc(fileName + ".gz")
            : File.Exists(fileName + ".zip") ? File.GetLastWriteTime(fileName + ".zip") : default;
        return ret;
    }


    public bool AutoUpdate { get; set; }
    public int HoursToUpdate { get; set; }
    [Column(TypeName = "citext")]
    public string? Url { get; set; }

}
