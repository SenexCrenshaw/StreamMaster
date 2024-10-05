using StreamMaster.Domain.Services;

namespace StreamMaster.Domain.Models;

public class JobStatusManager(IJobStatusService jobStatusService, JobType jobType, int id)
{

    public void Start()
    {
        jobStatusService.SetIsRunning(Key, true);
    }

    public void SetSuccessful()
    {
        jobStatusService.SetSuccessful(Key);
    }

    public void SetError()
    {
        jobStatusService.SetError(Key);
    }

    public bool IsRunning => jobStatusService.IsRunning(Key);
    public bool IsErrored => jobStatusService.IsErrored(Key);
    public bool ForceNextRun => jobStatusService.ForceNextRun(Key);
    public DateTime LastRun => jobStatusService.LastRun(Key);
    public DateTime LastSuccessful => jobStatusService.LastSuccessful(Key);

        public string JobType=> jobType.ToString();
    public void SetForceNextRun(bool extra = false)
    {
        jobStatusService.SetForceNextRun(Key, extra);
    }

    public void ClearForce()
    {
        jobStatusService.ClearForce(Key);
    }

    public JobStatus Status => jobStatusService.GetStatus(Key);

    private string Key => $"{jobType}:{id}";
}
