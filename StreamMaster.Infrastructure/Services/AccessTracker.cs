using StreamMaster.Domain.Services;

using System.Collections.Concurrent;

public class AccessTracker : IAccessTracker
{
    private readonly ConcurrentDictionary<string, StreamAccessInfo> _accessTimes = new();
    public TimeSpan CheckInterval { get; set; } = TimeSpan.FromMilliseconds(100); // Default, global check interval

    public void UpdateAccessTime(string videoStreamId, TimeSpan? inactiveThreshold = null)
    {
        StreamAccessInfo accessInfo = new()
        {
            LastAccessTime = DateTime.UtcNow,
            InactiveThreshold = inactiveThreshold ?? TimeSpan.FromSeconds(30) // Default threshold if not specified
        };

        _accessTimes.AddOrUpdate(videoStreamId, accessInfo, (key, existingVal) =>
        {
            existingVal.LastAccessTime = DateTime.UtcNow; // Update time
            if (inactiveThreshold.HasValue)
            {
                existingVal.InactiveThreshold = inactiveThreshold.Value; // Update threshold if provided
            }

            return existingVal;
        });
    }

    public IEnumerable<string> GetInactiveStreams()
    {
        DateTime now = DateTime.UtcNow;
        foreach (KeyValuePair<string, StreamAccessInfo> kvp in _accessTimes)
        {
            if (now - kvp.Value.LastAccessTime > kvp.Value.InactiveThreshold)
            {
                yield return kvp.Key;
            }
        }
    }

    public void Remove(string videoStreamId)
    {
        _accessTimes.TryRemove(videoStreamId, out _);
    }
}
