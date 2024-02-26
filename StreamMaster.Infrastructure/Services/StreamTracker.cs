using StreamMaster.Domain.Services;

using System.Collections.Concurrent;

namespace StreamMaster.Infrastructure.Services;

public class StreamTracker : IStreamTracker
{
    private static readonly ConcurrentDictionary<string, bool> streamTracker = new();
    public bool AddStream(string streamId)
    {
        return streamTracker.TryAdd(streamId, true);
    }

    public bool RemoveStream(string streamId)
    {
        return streamTracker.TryRemove(streamId, out _);
    }

    public bool HasStream(string streamId)
    {
        return streamTracker.ContainsKey(streamId);
    }

}
