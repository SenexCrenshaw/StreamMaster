using StreamMaster.Domain.Models;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Domain;

public class CacheManager() : ICacheManager
{
    public ConcurrentDictionary<int, IChannelBroadcaster> ChannelBroadcasters { get; private set; } = new ConcurrentDictionary<int, IChannelBroadcaster>();
    public ConcurrentDictionary<int, int> M3UMaxStreamCounts { get; private set; } = new ConcurrentDictionary<int, int>();
    public SMStreamInfo? MessageNoStreamsLeft { get; set; }
}
