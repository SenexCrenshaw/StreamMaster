using StreamMaster.Domain.Models;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Domain;

public class CacheManager() : ICacheManager
{
    public ConcurrentDictionary<int, IChannelBroadcaster> ChannelBroadcasters { get; } = new ConcurrentDictionary<int, IChannelBroadcaster>();
    public ConcurrentDictionary<int, int> M3UMaxStreamCounts { get; } = new ConcurrentDictionary<int, int>();
    public SMStreamInfo? MessageNoStreamsLeft { get; set; }

    public ConcurrentDictionary<int, string?> StreamGroupKeyCache { get; } = new();

}
