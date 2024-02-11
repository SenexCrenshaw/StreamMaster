using StreamMaster.Domain.Services;

namespace StreamMaster.Domain.Models;

public class JobStatusManager(IJobStatusService jobStatusService, JobType jobType, int id)
{

    public void Start()
    {
        jobStatusService.SetIsRunning(GenerateKey(), true);
    }

    //public void Stop()
    //{
    //    jobStatusService.SetIsRunning(GenerateKey(), false);
    //}

    public void SetSuccessful()
    {
        jobStatusService.SetSuccessful(GenerateKey());
    }

    public void SetError()
    {
        jobStatusService.SetError(GenerateKey());
    }

    public bool IsRunning => jobStatusService.IsRunning(GenerateKey());
    public bool ForceNextRun => jobStatusService.ForceNextRun(GenerateKey());
    public DateTime LastRun => jobStatusService.LastRun(GenerateKey());
    public DateTime LastSuccessful => jobStatusService.LastSuccessful(GenerateKey());


    public void SetForceNextRun(bool extra = false)
    {
        jobStatusService.SetForceNextRun(GenerateKey(), extra);
    }

    public void ClearForce()
    {
        jobStatusService.ClearForce(GenerateKey());
    }

    public JobStatus Status => jobStatusService.GetStatus(GenerateKey());

    private string GenerateKey()
    {
        return $"{jobType}:{id}";

    }
}
