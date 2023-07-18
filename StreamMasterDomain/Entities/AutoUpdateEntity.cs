namespace StreamMasterDomain.Entities;

public class AutoUpdateEntity : CacheEntity
{
    public AutoUpdateEntity()
    {
        HoursToUpdate = 72;
        AutoUpdate = true;
    }

    public bool AutoUpdate { get; set; }
    public int HoursToUpdate { get; set; }
    public string Description { get; set; } = string.Empty;
}
