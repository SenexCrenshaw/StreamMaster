using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Services;

public partial class JobStatusService : IJobStatusService
{

    private static readonly string RefreshM3USyncKey = "RefreshM3USync";

    public void SetM3USuccessful()
    {
        SetSuccessful(RefreshM3USyncKey);
    }

    public void SetM3UError()
    {
        SetError(RefreshM3USyncKey);
    }

    public void SetM3UForceNextRun(bool Extra = false)
    {
        SetForceNextRun(RefreshM3USyncKey, Extra);
    }

    public void SetM3UIsRunning(bool isRunning)
    {
        SetIsRunning(RefreshM3USyncKey, isRunning);
    }

    public JobStatus GetM3UJobStatus()
    {
        return GetStatus(RefreshM3USyncKey);
    }

    public void ClearM3UForce()
    {
        ClearForce(RefreshM3USyncKey);
    }


}
