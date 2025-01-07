using System.Collections.Concurrent;

namespace StreamMaster.WebDav.Providers;

/// <summary>
/// In-memory implementation of ILockManager.
/// </summary>
public class LockManager : ILockManager
{
    private readonly ConcurrentDictionary<string, (string Token, string Owner, DateTime Expiry)> _locks = new();

    public async Task<string?> AcquireLockAsync(string resourcePath, string owner, bool isExclusive, TimeSpan timeout, CancellationToken cancellationToken)
    {
        string lockToken = Guid.NewGuid().ToString();
        DateTime expiry = DateTime.UtcNow.Add(timeout);

        if (!_locks.TryAdd(resourcePath, (lockToken, owner, expiry)))
        {
            return null; // Resource is already locked.
        }

        return lockToken;
    }

    public async Task<bool> ReleaseLockAsync(string lockToken, CancellationToken cancellationToken)
    {
        foreach (string key in _locks.Keys)
        {
            if (_locks.TryGetValue(key, out (string Token, string Owner, DateTime Expiry) lockInfo) && lockInfo.Token == lockToken)
            {
                return _locks.TryRemove(key, out _);
            }
        }
        return false;
    }

    public async Task<bool> IsLockedAsync(string resourcePath, CancellationToken cancellationToken)
    {
        return _locks.TryGetValue(resourcePath, out (string Token, string Owner, DateTime Expiry) lockInfo) && lockInfo.Expiry > DateTime.UtcNow;
    }
}