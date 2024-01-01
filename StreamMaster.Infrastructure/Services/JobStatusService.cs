using StreamMaster.Domain.Services;

using System.Collections.Concurrent;

namespace StreamMaster.Infrastructure.Services;

public class JobStatusService : IJobStatusService
{
    private static readonly string EPGSyncKey = "EPGSync";


    private readonly ConcurrentDictionary<string, JobStatus> _jobs = [];
    private readonly ConcurrentDictionary<string, object> _locks = [];

    private JobStatus GetStatus(string key)
    {
        if (!_jobs.TryGetValue(key, out JobStatus? status))
        {
            status = new();
            SetStatus(key, status);
        }

        return status!;

    }

    private void SetStatus(string key, JobStatus status)
    {
        _ = _jobs.AddOrUpdate(key, status, (key, value) => value = status);
    }

    private void UpdateStatus(string key, Action<JobStatus> updateAction, object lockObject)
    {
        lock (lockObject)
        {
            JobStatus status = GetStatus(key);
            updateAction(status);
            SetStatus(key, status);
        }
    }

    private void SetSuccessful(string key)
    {
        UpdateStatus(key, status => status.SetSuccessful(), _locks.GetOrAdd(key, new object()));
    }

    private void SetError(string key)
    {
        UpdateStatus(key, status => status.SetError(), _locks.GetOrAdd(key, new object()));
    }

    private void SetForceNextRun(string key, bool Extra = false)
    {
        UpdateStatus(key, status => status.SetForceNextRun(Extra), _locks.GetOrAdd(key, new object()));
    }

    private void SetIsRunning(string key, bool isRunning)
    {
        UpdateStatus(key, status => status.SetIsRunning(isRunning), _locks.GetOrAdd(key, new object()));
    }

    private void ClearForce(string key)
    {
        UpdateStatus(key, status => status.SetForceNextRun(false), _locks.GetOrAdd(key, new object()));
    }

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
