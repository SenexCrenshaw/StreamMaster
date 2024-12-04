
using StreamMaster.Domain.API;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

public interface ISchedulesDirect
{
    void RemovedExpiredKeys();
    void ResetAllEPGCaches();
    Task<APIResponse> SDSync(CancellationToken cancellationToken);
}
