using System.Collections.Concurrent;

namespace StreamMaster.Streams.Domain;

public class CacheManager() : ICacheManager
{
    public ConcurrentDictionary<int, IChannelStatus> ChannelStatuses { get; private set; } = new ConcurrentDictionary<int, IChannelStatus>();
    public ConcurrentDictionary<int, int> M3UMaxStreamCounts { get; private set; } = new ConcurrentDictionary<int, int>();

}
