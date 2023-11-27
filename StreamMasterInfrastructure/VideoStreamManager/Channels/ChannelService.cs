using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Dto;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager.Channels;

public class ChannelService(IClientStreamerManager clientManager) : IChannelService
{
    private readonly ConcurrentDictionary<string, IChannelStatus> _channelStatuses = new();

    public IChannelStatus RegisterChannel(VideoStreamDto ChannelVideoStream, string ChannelName)
    {

        if (!_channelStatuses.TryGetValue(ChannelVideoStream.Id, out IChannelStatus? channelStatus))
        {
            channelStatus = new ChannelStatus(ChannelVideoStream.Id, ChannelVideoStream.User_Tvg_name, ChannelName);
            _ = _channelStatuses.TryAdd(ChannelVideoStream.Id, channelStatus);
        }

        channelStatus.CurrentVideoStream = ChannelVideoStream;

        return channelStatus;
    }

    public void UnRegisterChannel(string channelVideoStreamId)
    {
        _ = _channelStatuses.TryRemove(channelVideoStreamId, out _);
    }

    public IChannelStatus? GetChannelStatus(string channelVideoStreamId)
    {
        _ = _channelStatuses.TryGetValue(channelVideoStreamId, out IChannelStatus? channelStatus);
        return channelStatus;
    }

    public List<IChannelStatus> GetChannelStatusesFromVideoStreamId(string VideoStreamId)
    {
        List<IChannelStatus> test = _channelStatuses.Values.Where(a => a.ChannelVideoStreamId == VideoStreamId || a.CurrentVideoStream.Id == VideoStreamId).ToList();

        return test;
    }

    public List<IChannelStatus> GetChannelStatuses()
    {
        return [.. _channelStatuses.Values];
    }

    public bool HasChannel(string channelVideoStreamId)
    {
        return _channelStatuses.ContainsKey(channelVideoStreamId);
    }

    public int GetGlobalStreamsCount()
    {
        return _channelStatuses.Count(a => a.Value.IsGlobal);
    }

    public async Task<IClientStreamerConfiguration?> GetClientStreamerConfiguration(Guid clientId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IClientStreamerConfiguration? test = await clientManager.GetClientStreamerConfiguration(clientId, cancellationToken);
        return test;
    }

    public List<IClientStreamerConfiguration> GetClientStreamerConfigurationFromIds(List<Guid> clientIds)
    {
        List<IClientStreamerConfiguration> test = clientManager.GetClientStreamerConfigurationFromIds(clientIds);
        return test;
    }

}