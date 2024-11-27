namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

public interface IEPGCached
{
    List<string> GetExpiredKeys();
    void RemovedExpiredKeys(List<string>? keysToDelete = null);
    void ClearCache();
    void ResetCache();
}