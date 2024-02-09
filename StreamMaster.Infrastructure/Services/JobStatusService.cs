using StreamMaster.Domain.Services;

using System.Collections.Concurrent;

namespace StreamMaster.Infrastructure.Services;

public partial class JobStatusService : IJobStatusService
{
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
        if (isRunning)
        {
            UpdateStatus(key, status => status.SetStart(), _locks.GetOrAdd(key, new object()));
        }
        else
        {
            UpdateStatus(key, status => status.SetStop(), _locks.GetOrAdd(key, new object()));
        }
    }

    private void ClearForce(string key)
    {
        UpdateStatus(key, status => status.SetForceNextRun(false), _locks.GetOrAdd(key, new object()));
    }


}
