using System.Collections.Concurrent;

using StreamMaster.Domain.Models;
using StreamMaster.Domain.XmltvXml;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface ICacheManager
{
    List<StationChannelName> GetStationChannelNames { get; }
    ConcurrentDictionary<int, (int channelCount, int programmeCount)> StationChannelCounts { get; }
    void ClearEPGDataByEPGNumber(int epgNumber);
    ConcurrentDictionary<int, List<StationChannelName>> StationChannelNames { get; }
    ConcurrentDictionary<int, string?> StreamGroupKeyCache { get; }
    ConcurrentDictionary<int, IChannelBroadcaster> ChannelBroadcasters { get; }
    ConcurrentDictionary<int, int> M3UMaxStreamCounts { get; }
    SMStreamInfo? MessageNoStreamsLeft { get; set; }
    StreamGroup? DefaultSG { get; set; }
}