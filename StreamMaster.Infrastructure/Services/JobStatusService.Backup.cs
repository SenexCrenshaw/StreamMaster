using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Services;

public partial class JobStatusService : IJobStatusService
{

    private static readonly string BackupSyncKey = "BackupSync";

    public void SetBackupSuccessful()
    {
        SetSuccessful(BackupSyncKey);
    }

    public void SetBackupError()
    {
        SetError(BackupSyncKey);
    }

    public void SetBackupForceNextRun(bool Extra = false)
    {
        SetForceNextRun(BackupSyncKey, Extra);
    }

    public void SetBackupStart()
    {
        SetIsRunning(BackupSyncKey, true);
    }

    public void SetBackupStop()
    {
        SetIsRunning(BackupSyncKey, false);
    }

    public JobStatus GetBackupJobStatus()
    {
        return GetStatus(BackupSyncKey);
    }

    public void ClearBackupForce()
    {
        ClearForce(BackupSyncKey);
    }


}
