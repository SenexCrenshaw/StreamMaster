using System.Collections.Concurrent;

using StreamMaster.Domain.Models;
using StreamMaster.Domain.XmltvXml;

namespace StreamMaster.Streams.Domain;

public class CacheManager() : ICacheManager
{
    public ConcurrentDictionary<int, IChannelBroadcaster> ChannelBroadcasters { get; } = new ConcurrentDictionary<int, IChannelBroadcaster>();
    public ConcurrentDictionary<int, int> M3UMaxStreamCounts { get; } = new ConcurrentDictionary<int, int>();
    public ConcurrentDictionary<int, List<StationChannelName>> StationChannelNames { get; } = new();
    public ConcurrentDictionary<int, (int channelCount, int programmeCount)> StationChannelCounts { get; } = new();

    public void ClearEPGDataByEPGNumber(int epgNumber)
    {
        StationChannelNames.TryRemove(epgNumber, out _);
        StationChannelCounts.TryRemove(epgNumber, out _);
    }

    public SMStreamInfo? MessageNoStreamsLeft { get; set; }
    public List<StationChannelName> GetStationChannelNames => [.. StationChannelNames.SelectMany(x => x.Value)];
    public ConcurrentDictionary<int, string?> StreamGroupKeyCache { get; } = new();
    public StreamGroup? DefaultSG { get; set; }
}
