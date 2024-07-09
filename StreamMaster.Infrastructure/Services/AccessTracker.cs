using StreamMaster.Domain.Models;
using StreamMaster.Domain.Services;

using System.Collections.Concurrent;

public class AccessTracker : IAccessTracker
{
    private readonly ConcurrentDictionary<int, StreamAccessInfo> _accessTimes = new();
    public TimeSpan CheckInterval { get; set; } = TimeSpan.FromMilliseconds(100); // Default, global check interval

    public void UpdateAccessTime(int smChannelId, TimeSpan? inactiveThreshold = null)
    {
        StreamAccessInfo accessInfo = new()
        {
            LastAccessTime = DateTime.UtcNow,
            InactiveThreshold = inactiveThreshold ?? TimeSpan.FromSeconds(30) // Default threshold if not specified
        };

        _accessTimes.AddOrUpdate(smChannelId, accessInfo, (key, existingVal) =>
        {
            existingVal.LastAccessTime = DateTime.UtcNow; // Update time
            if (inactiveThreshold.HasValue)
            {
                existingVal.InactiveThreshold = inactiveThreshold.Value; // Update threshold if provided
            }

            return existingVal;
        });
    }

    public IEnumerable<int> GetInactiveStreams()
    {
        DateTime now = DateTime.UtcNow;
        foreach (KeyValuePair<int, StreamAccessInfo> kvp in _accessTimes)
        {
            if (now - kvp.Value.LastAccessTime > kvp.Value.InactiveThreshold)
            {
                yield return kvp.Key;
            }
        }
    }

    public void Remove(int SMChannelId)
    {
        _accessTimes.TryRemove(SMChannelId, out _);
    }
}
