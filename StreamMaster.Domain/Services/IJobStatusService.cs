namespace StreamMaster.Domain.Services
{
    public interface IJobStatusService
    {
        JobStatusManager GetJobManagerProcessM3U(int id);
        DateTime LastRun(string key);
        DateTime LastSuccessful(string key);
        JobStatusManager GetJobManager(JobType jobType, int id);
        bool IsRunning(string key);
        bool ForceNextRun(string key);
        void SetIsRunning(string key, bool isRunning);
        void SetSuccessful(string key);
        void SetError(string key);
        void ClearForce(string key);
        void SetForceNextRun(string key, bool extra = false);
        JobStatus GetStatus(string key);
    }
}