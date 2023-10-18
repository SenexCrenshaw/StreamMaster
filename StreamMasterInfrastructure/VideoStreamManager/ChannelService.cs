using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager;
public class ChannelService : IChannelService
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

    public ClientStreamerConfiguration? GetClientStreamerConfiguration(Guid clientId)
    {
        List<ClientStreamerConfiguration> test = _channelStatuses.Values.SelectMany(a => a.GetChannelClientClientStreamerConfigurations.Where(a => a.ClientId == clientId)).ToList();
        if (test.Any())
        {
            return test.First();
        }
        return null;
    }

    public List<ClientStreamerConfiguration> GetClientStreamerConfigurations()
    {
        List<ClientStreamerConfiguration> test = _channelStatuses.Values.SelectMany(a => a.GetChannelClientClientStreamerConfigurations).ToList();
        return test;
    }

    public List<ClientStreamerConfiguration> GetClientStreamerConfigurationFromIds(List<Guid> clientIds)
    {
        List<ClientStreamerConfiguration> test = _channelStatuses.Values.SelectMany(a => a.GetChannelClientClientStreamerConfigurations.Where(a => clientIds.Contains(a.ClientId))).ToList();
        return test;
    }
}