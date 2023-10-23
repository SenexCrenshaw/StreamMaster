using StreamMasterApplication.Common.Interfaces;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager.Channels;

public class ChannelService(IClientStreamerManager clientManager) : IChannelService
{
    private readonly ConcurrentDictionary<string, IChannelStatus> _channelStatuses = new();

    public IChannelStatus RegisterChannel(string channelVideoStreamId, string videoStreamName)
    {
        if (!_channelStatuses.TryGetValue(channelVideoStreamId, out IChannelStatus? channelStatus))
        {
            channelStatus = new ChannelStatus(channelVideoStreamId, videoStreamName);
            _channelStatuses.TryAdd(channelVideoStreamId, channelStatus);
        }

        return channelStatus;
    }

    public void UnregisterChannel(string channelVideoStreamId)
    {
        _channelStatuses.TryRemove(channelVideoStreamId, out _);
    }

    public IChannelStatus? GetChannelStatus(string channelVideoStreamId)
    {
        _channelStatuses.TryGetValue(channelVideoStreamId, out IChannelStatus? channelStatus);
        return channelStatus;
    }

    public List<IChannelStatus> GetChannelStatuses()
    {
        return _channelStatuses.Values.ToList();
    }

    public bool HasChannel(string channelVideoStreamId)
    {
        return _channelStatuses.ContainsKey(channelVideoStreamId);
    }

    public int GetGlobalStreamsCount()
    {
        return _channelStatuses.Count(a => a.Value.IsGlobal);
    }

    public IClientStreamerConfiguration? GetClientStreamerConfiguration(Guid clientId)
    {
        var test = clientManager.GetClientStreamerConfiguration(clientId);
        return test;
    }

    public List<IClientStreamerConfiguration> GetClientStreamerConfigurationFromIds(List<Guid> clientIds)
    {
        List<IClientStreamerConfiguration> test = clientManager.GetClientStreamerConfigurationFromIds(clientIds);
        return test;
    }
}