namespace StreamMaster.SchedulesDirectAPI.Services;

public class SDTokenCache : ISDTokenCache
{
    private (ISDStatus? status, DateTime timestamp)? cacheEntry = null;

    public ISDStatus? GetStatus()
    {
        if (cacheEntry.HasValue && (DateTime.UtcNow - cacheEntry.Value.timestamp).TotalMinutes < 10)
        {
            return cacheEntry.Value.status;
        }
        return null;
    }

    public void SetStatus(ISDStatus status)
    {
        cacheEntry = (status, DateTime.UtcNow);
    }
}