using StreamMaster.Domain.Extensions;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Services
{
    public class AccessTracker : IAccessTracker
    {
        private readonly ConcurrentDictionary<string, StreamAccessInfo> _accessTimes = new();
        public TimeSpan CheckInterval { get; set; } = TimeSpan.FromMilliseconds(100); // Default, global check interval

        public StreamAccessInfo UpdateAccessTime(string key, string smStreamId, TimeSpan? inactiveThreshold = null)
        {
            DateTime now = SMDT.UtcNow;
            StreamAccessInfo accessInfo = new()
            {
                Key = key,
                SMStreamId = smStreamId,
                LastAccessTime = now,
                MillisecondsSinceLastUpdate = 0,
                InactiveThreshold = inactiveThreshold ?? TimeSpan.FromSeconds(10) // Default threshold if not specified
            };

            StreamAccessInfo result = _accessTimes.AddOrUpdate(key, accessInfo, (_, existingVal) =>
            {
                DateTime previousAccessTime = existingVal.LastAccessTime;
                existingVal.LastAccessTime = now;
                if (inactiveThreshold.HasValue)
                {
                    existingVal.InactiveThreshold = inactiveThreshold.Value;
                }

                existingVal.MillisecondsSinceLastUpdate = (existingVal.LastAccessTime - previousAccessTime).TotalMilliseconds;
                return existingVal;
            });

            return result;
        }

        public IEnumerable<StreamAccessInfo> GetInactiveStreams()
        {
            DateTime adjustedNow = SMDT.UtcNow;
            foreach (KeyValuePair<string, StreamAccessInfo> kvp in _accessTimes)
            {
                TimeSpan span = adjustedNow - kvp.Value.LastAccessTime;
                if (span > kvp.Value.InactiveThreshold)
                {
                    kvp.Value.MillisecondsSinceLastUpdate = span.TotalMilliseconds;
                    yield return kvp.Value;
                }
            }
        }

        public void RemoveAccessTime(string key)
        {
            _accessTimes.TryRemove(key, out _);
        }
    }
}
