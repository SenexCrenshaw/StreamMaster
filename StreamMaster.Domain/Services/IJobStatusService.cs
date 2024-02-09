using StreamMaster.Domain.Models;

namespace StreamMaster.Domain.Services
{
    public interface IJobStatusService
    {
        void ClearSyncForce();
        JobStatus GetSyncJobStatus();
        void SetSyncError();
        void SetSyncForceNextRun(bool Extra = false);
        void SetSyncStop();
        void SetSyncStart();
        void SetSyncSuccessful();


        // M3U
        void SetM3USuccessful();
        void SetM3UError();
        void SetM3UForceNextRun(bool Extra = false);
        void SetM3UStop();
        void SetM3UStart();
        JobStatus GetM3UJobStatus();
        void ClearM3UForce();


        // EPG
        void SetEPGSuccessful();
        void SetEPGError();
        void SetEPGForceNextRun(bool Extra = false);
        void SetEPGStop();
        void SetEPGStart();
        JobStatus GetEPGJobStatus();
        void ClearEPGForce();

        // Backup
        void SetBackupSuccessful();
        void SetBackupError();
        void SetBackupForceNextRun(bool Extra = false);
        void SetBackupStop();
        void SetBackupStart();
        JobStatus GetBackupJobStatus();
        void ClearBackupForce();
    }
}