using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Configuration;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Channels;

public sealed class ChannelService(ILogger<ChannelService> logger, IServiceProvider serviceProvider, IOptionsMonitor<Setting> intsettings) : IChannelService
{
    private readonly Setting settings = intsettings.CurrentValue;

    private readonly ConcurrentDictionary<int, IChannelStatus> _channelStatuses = new();

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

    public async Task<IChannelStatus?> RegisterChannel(SMChannel smChannel, bool fetch = false)
    {
        IChannelStatus? channelStatus = GetChannelStatus(smChannel.Id);
        if (channelStatus == null)
        {
            channelStatus = new ChannelStatus(smChannel);
            _channelStatuses.TryAdd(smChannel.Id, channelStatus);
            if (!fetch)
            {
                return channelStatus;
            }

            await SetNextChildVideoStream(smChannel.Id);
        }

        return channelStatus;
    }

    public void UnRegisterChannel(string smChannelId)
    {
        _ = _channelStatuses.TryRemove(smChannelId, out _);
    }

    public IChannelStatus? GetChannelStatus(int smChannelId)
    {
        _ = _channelStatuses.TryGetValue(smChannelId, out IChannelStatus? channelStatus);
        return channelStatus;
    }

    public List<IChannelStatus> GetChannelStatusesFromSMStreamId(string SMStreamId)
    {
        List<IChannelStatus> test = _channelStatuses.Values.ToList();
        return _channelStatuses.Values.Where(a => a.ChannelSMStreamId == SMStreamId || a.CurrentVideoStream.Id == SMStreamId).ToList();
    }

    public List<IChannelStatus> GetChannelStatuses()
    {
        return [.. _channelStatuses.Values];
    }

    public bool HasChannel(string SMStreamId)
    {
        return _channelStatuses.ContainsKey(SMStreamId);
    }

    public int GetGlobalStreamsCount()
    {
        return _channelStatuses.Count(a => a.Value.IsGlobal);
    }

    private async Task SetNextChildVideoStream(int channelSMStreamId)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

        IChannelStatus? channelStatus = GetChannelStatus(channelSMStreamId);

        SMChannel? smChannel = repository.SMChannel.GetSMChannel(channelSMStreamId);

        (VideoStreamHandlers videoStreamHandler, List<SMStreamDto> childVideoStreamDtos)? result = await repository.SMChannel.GetStreamsFromVideoStreamById(channelStatus.channelSMStreamId);
        if (result == null)
        {
            logger.LogError("SetNextChildVideoStream could not get videoStreams for id {ParentVideoStreamId}", channelStatus.channelSMStreamId);
            logger.LogDebug("Exiting SetNextChildVideoStream with null due to result being null");
            channelStatus.SetCurrentVideoStream(null);
            return;
        }

        VideoStreamDto[] videoStreams = [.. result.Value.childVideoStreamDtos.OrderBy(a => a.Rank)];
        if (!videoStreams.Any())
        {
            channelStatus.SetCurrentVideoStream(null);
            return;
        }

        VideoStreamHandlers videoHandler = result.Value.videoStreamHandler == VideoStreamHandlers.SystemDefault ? VideoStreamHandlers.Loop : result.Value.videoStreamHandler;

        if (channelStatus.Rank >= videoStreams.Length)
        {
            channelStatus.Rank = 0;
        }

        while (channelStatus.Rank < videoStreams.Length)
        {
            VideoStreamDto toReturn = videoStreams[channelStatus.Rank++];
            List<M3UFileDto> m3uFilesRepo = await repository.M3UFile.GetM3UFiles().ConfigureAwait(false);

            M3UFileDto? m3uFile = m3uFilesRepo.Find(a => a.Id == toReturn.M3UFileId);
            if (m3uFile == null)
            {
                if (GetGlobalStreamsCount() >= settings.GlobalStreamLimit)
                {
                    logger.LogInformation("Max Global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), toReturn.User_Url);
                    continue;
                }

                channelStatus.SetIsGlobal();
                logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
            }
            else
            {
                int allStreamsCount = 0;// streamManager.GetStreamsCountForM3UFile(toReturn.M3UFileId);
                if (allStreamsCount >= m3uFile.MaxStreamCount)
                {
                    logger.LogInformation("Max stream count {MaxStreams} reached for stream: {StreamUrl}", toReturn.MaxStreams, toReturn.User_Url);
                    continue;
                }
            }
            logger.LogDebug("Exiting SetNextChildVideoStream with to Return: {Id} {Name}", toReturn.Id, toReturn.User_Tvg_name);

            channelStatus.SetCurrentVideoStream(toReturn);

            return;
        }

        logger.LogDebug("Exiting SetNextChildVideoStream with null due to no suitable videoStream found");
        channelStatus.SetCurrentVideoStream(null);
        return;
    }

}