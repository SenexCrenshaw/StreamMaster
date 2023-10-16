using StreamMasterApplication.Common.Interfaces;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager;
public class ChannelService : IChannelService
{
    private readonly ConcurrentDictionary<string, IChannelStatus> _channelStatuses = new();

    public IChannelStatus RegisterChannel(string videoStreamId, string videoStreamName)
    {
        if (!_channelStatuses.TryGetValue(videoStreamId, out IChannelStatus? channelStatus))
        {
            channelStatus = new ChannelStatus(videoStreamId, videoStreamName);
            _channelStatuses.TryAdd(videoStreamId, channelStatus);
        }

        return channelStatus;
    }

    public void UnregisterChannel(string videoStreamId)
    {
        _channelStatuses.TryRemove(videoStreamId, out _);
    }

    public IChannelStatus? GetChannelStatus(string videoStreamId)
    {
        _channelStatuses.TryGetValue(videoStreamId, out IChannelStatus? channelStatus);
        return channelStatus;
    }

    public List<IStreamHandler> GetStreamHandlers()
    {
        if (_channelStatuses.Values == null)
        {
            return new List<IStreamHandler>();
        }

        return _channelStatuses.Values.Where(a => a?.StreamHandler != null).Select(a => a.StreamHandler).ToList();
    }

    public bool HasChannel(string videoStreamId)
    {
        return _channelStatuses.ContainsKey(videoStreamId);
    }

    public int GetGlobalStreamsCount()
    {
        return _channelStatuses.Count(a => a.Value.IsGlobal);
    }
}