using System.Collections.Concurrent;

namespace StreamMaster.Infrastructure.Services;

public class StreamTracker : IStreamTracker
{
    private static readonly ConcurrentDictionary<int, bool> streamTracker = new();
    public bool AddStream(int smChannelId)
    {
        return streamTracker.TryAdd(smChannelId, true);
    }

    public bool RemoveStream(int smChannelId)
    {
        return streamTracker.TryRemove(smChannelId, out _);
    }

    public bool HasStream(int smChannelId)
    {
        return streamTracker.ContainsKey(smChannelId);
    }

}
