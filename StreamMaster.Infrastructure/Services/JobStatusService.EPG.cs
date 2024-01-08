using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Services;

public partial class JobStatusService : IJobStatusService
{

    private static readonly string RefreshEPGSyncKey = "RefreshEPGSync";

    public void SetEPGSuccessful()
    {
        SetSuccessful(RefreshEPGSyncKey);
    }

    public void SetEPGError()
    {
        SetError(RefreshEPGSyncKey);
    }

    public void SetEPGForceNextRun(bool Extra = false)
    {
        SetForceNextRun(RefreshEPGSyncKey, Extra);
    }

    public void SetEPGIsRunning(bool isRunning)
    {
        SetIsRunning(RefreshEPGSyncKey, isRunning);
    }

    public JobStatus GetEPGJobStatus()
    {
        return GetStatus(RefreshEPGSyncKey);
    }

    public void ClearEPGForce()
    {
        ClearForce(RefreshEPGSyncKey);
    }


}
