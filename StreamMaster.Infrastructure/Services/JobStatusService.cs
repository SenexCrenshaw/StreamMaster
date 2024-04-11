using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Services;

using System.Collections.Concurrent;

namespace StreamMaster.Infrastructure.Services;

public partial class JobStatusService(ILogger<JobStatusService> logger) : IJobStatusService
{
    public readonly ConcurrentDictionary<string, JobStatus> _jobs = [];
    public readonly ConcurrentDictionary<string, object> _locks = [];
    public JobStatusManager GetJobManagerProcessM3U(int id)
    {
        return new JobStatusManager(this, JobType.ProcessM3U, id);
    }

    public JobStatusManager GetJobManagerProcessEPG(int id)
    {
        return new JobStatusManager(this, JobType.ProcessEPG, id);
    }

    public JobStatusManager GetJobManager(JobType jobType, int id)
    {
        return new JobStatusManager(this, jobType, id);
    }

    public JobStatus GetStatus(string key)
    {
        if (!_jobs.TryGetValue(key, out JobStatus? status))
        {
            status = new();
            SetStatus(key, status);
        }

        return status!;

    }

    public void SetStatus(string key, JobStatus status)
    {
        try
        {
            _jobs.AddOrUpdate(key, addValueFactory: _ => status, updateValueFactory: (_, _) => status);
            //logger.LogInformation("Status for {JobKey} has been updated successfully.", key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update status for {JobKey}.", key);
            throw;
        }
    }

    public void UpdateStatus(string key, Action<JobStatus> updateAction)
    {
        object lockObject = _locks.GetOrAdd(key, _ => new object());
        lock (lockObject)
        {
            try
            {
                JobStatus status = GetStatus(key);
                updateAction(status);
                SetStatus(key, status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating status for {JobKey}.", key);
                throw; // Rethrow if you want the exception to be handled further up the call stack
            }
        }
    }

    public void SetSuccessful(string key)
    {
        UpdateStatus(key, status => status.SetSuccessful());
    }

    public void SetError(string key)
    {
        UpdateStatus(key, status => status.SetError());
    }

    public void SetForceNextRun(string key, bool Extra = false)
    {
        UpdateStatus(key, status => status.SetForceNextRun(Extra));
    }

    public void SetIsRunning(string key, bool isRunning)
    {
        if (isRunning)
        {
            UpdateStatus(key, status => status.SetStart());
        }
        else
        {
            UpdateStatus(key, status => status.SetStop());
        }
    }

    public void ClearForce(string key)
    {
        UpdateStatus(key, status => status.SetForceNextRun(false));
    }

    public bool IsRunning(string key)
    {
        return GetStatus(key).IsRunning;
    }

    public bool ForceNextRun(string key)
    {
        return GetStatus(key).ForceNextRun;
    }

    public DateTime LastRun(string key)
    {
        return GetStatus(key).LastRun;
    }

    public DateTime LastSuccessful(string key)
    {
        return GetStatus(key).LastSuccessful;
    }

    public JobStatusManager GetJobManagerRefreshEPG(int id)
    {
        return new JobStatusManager(this, JobType.RefreshEPG, id);
    }

    public JobStatusManager GetJobManagerUpdateEPG(int id)
    {
        return new JobStatusManager(this, JobType.UpdateEPG, id);
    }

    public JobStatusManager GetJobManagerUpdateM3U(int id)
    {
        return new JobStatusManager(this, JobType.UpdateM3U, id);
    }

    public JobStatusManager GetJobManagerRefreshM3U(int id)
    {
        return new JobStatusManager(this, JobType.RefreshM3U, id);
    }
}
