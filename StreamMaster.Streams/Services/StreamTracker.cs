using System.Collections.Concurrent;

namespace StreamMaster.Streams.Services;

public class StreamTracker : IStreamTracker
{
    private static readonly ConcurrentDictionary<string, bool> streamTracker = new();
    public bool AddStream(string smStreamId)
    {
        return streamTracker.TryAdd(smStreamId, true);
    }

    public bool RemoveStream(string smStreamId)
    {
        return streamTracker.TryRemove(smStreamId, out _);
    }

    public bool HasStream(string smStreamId)
    {
        return streamTracker.ContainsKey(smStreamId);
    }
}
