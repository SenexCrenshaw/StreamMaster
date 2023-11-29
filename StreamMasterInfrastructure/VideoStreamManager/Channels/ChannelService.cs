using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Dto;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager.Channels;

public sealed class ChannelService() : IChannelService
{
    private readonly ConcurrentDictionary<string, IChannelStatus> _channelStatuses = new();

    private readonly object _disposeLock = new();
    private bool _disposed = false;

    public void Dispose()
    {
        lock (_disposeLock)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                _channelStatuses.Clear();

            }
            catch { }
            finally
            {
                _disposed = true;
            }
        }

    }

    public IChannelStatus RegisterChannel(VideoStreamDto ChannelVideoStream)
    {

        if (!_channelStatuses.TryGetValue(ChannelVideoStream.Id, out IChannelStatus? channelStatus))
        {
            channelStatus = new ChannelStatus(ChannelVideoStream);
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
        return _channelStatuses.Values.Where(a => a.ChannelVideoStreamId == VideoStreamId || a.CurrentVideoStream.Id == VideoStreamId).ToList();
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

}