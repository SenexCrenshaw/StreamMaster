using StreamMaster.Domain.Models;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface ICacheManager
{
    ConcurrentDictionary<int, IChannelStatus> ChannelStatuses { get; }
    ConcurrentDictionary<int, int> M3UMaxStreamCounts { get; }
    SMStreamInfo? MessageNoStreamsLeft { get; set; }
}