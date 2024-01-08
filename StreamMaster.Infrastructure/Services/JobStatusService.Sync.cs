using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Services;

public partial class JobStatusService : IJobStatusService
{
    private static readonly string EPGSyncKey = "EPGSync";


    public void SetSyncSuccessful()
    {
        SetSuccessful(EPGSyncKey);
    }

    public void SetSyncError()
    {
        SetError(EPGSyncKey);
    }

    public void SetSyncForceNextRun(bool Extra = false)
    {
        SetForceNextRun(EPGSyncKey, Extra);
    }

    public void SetSyncIsRunning(bool isRunning)
    {
        SetIsRunning(EPGSyncKey, isRunning);
    }

    public JobStatus GetSyncJobStatus()
    {
        return GetStatus(EPGSyncKey);
    }

    public void ClearSyncForce()
    {
        ClearForce(EPGSyncKey);
    }


}
