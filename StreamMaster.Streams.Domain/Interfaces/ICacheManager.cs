using StreamMaster.Domain.Models;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface ICacheManager
{
    ConcurrentDictionary<int, string?> StreamGroupKeyCache { get; }
    ConcurrentDictionary<int, IChannelBroadcaster> ChannelBroadcasters { get; }
    ConcurrentDictionary<int, int> M3UMaxStreamCounts { get; }
    SMStreamInfo? MessageNoStreamsLeft { get; set; }
}