using StreamMaster.Domain.Models;

namespace StreamMaster.Domain.Services
{
    public interface IJobStatusService
    {
        void ClearSyncForce();
        JobStatus GetSyncJobStatus();
        void SetSyncError();
        void SetSyncForceNextRun(bool Extra = false);
        void SetSyncIsRunning(bool v);
        void SetSyncSuccessful();
    }
}