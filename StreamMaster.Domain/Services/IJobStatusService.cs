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


        // M3U
        void SetM3USuccessful();
        void SetM3UError();
        void SetM3UForceNextRun(bool Extra = false);
        void SetM3UIsRunning(bool v);
        JobStatus GetM3UJobStatus();
        void ClearM3UForce();


        // EPG
        void SetEPGSuccessful();
        void SetEPGError();
        void SetEPGForceNextRun(bool Extra = false);
        void SetEPGIsRunning(bool v);
        JobStatus GetEPGJobStatus();
        void ClearEPGForce();
    }
}