using MediatR;

using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Configuration;

using System.Collections.Concurrent;

public sealed class ChannelService(
    ILogger<ChannelService> logger,
    IStreamManager streamManager,
    IClientStreamerManager clientStreamerManager,
    IOptionsMonitor<VideoOutputProfiles> intprofilesettings,
    ISender sender,
    IServiceProvider serviceProvider,
    IOptionsMonitor<Setting> settingsMonitor
    ) : IChannelService, IDisposable
{
    private readonly ILogger<ChannelService> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly Setting _settings = settingsMonitor.CurrentValue ?? throw new ArgumentNullException(nameof(settingsMonitor));
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
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while disposing the ChannelService.");
            }
            finally
            {
                _disposed = true;
            }
        }
    }

    private VideoOutputProfileDto VideoOutputProfileDto(string StreamingProxyType)
    {
        if (intprofilesettings.CurrentValue.VideoProfiles.TryGetValue(StreamingProxyType, out VideoOutputProfile videoOutputProfile))
        {
            return new VideoOutputProfileDto
            {
                Command = videoOutputProfile.Command,
                ProfileName = StreamingProxyType,
                IsReadOnly = videoOutputProfile.IsReadOnly,
                Parameters = videoOutputProfile.Parameters,
                Timeout = videoOutputProfile.Timeout,
                IsM3U8 = videoOutputProfile.IsM3U8
            };
        }

        return new VideoOutputProfileDto
        {
            ProfileName = StreamingProxyType,
        };



    }
    public async Task<IChannelStatus?> RegisterChannel(IClientStreamerConfiguration config)
    {


        if (config.SMChannel == null)
        {
            throw new ArgumentNullException(nameof(config.SMChannel));
        }

        IChannelStatus? channelStatus = GetChannelStatus(config.SMChannel.Id);

        clientStreamerManager.RegisterClient(config);

        if (channelStatus != null)
        {
            IStreamHandler? handler = streamManager.GetStreamHandler(channelStatus.SMStream.Url);
            if (handler is null)
            {
                logger.LogError("Could not find handler for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.SMChannel.Id, config.SMChannel.Name);
                return null;
            }
            channelStatus.VideoProfile = VideoOutputProfileDto(config.SMChannel.StreamingProxyType);

            if (handler.IsFailed)
            {
                logger.LogInformation("Existing handler is failed, creating");

                await SwitchChannelToNextStream(channelStatus);
            }

            await streamManager.AddClientToHandler(channelStatus.SMChannel, config, handler);
            logger.LogInformation("Reuse existing stream handler for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.SMChannel.Id, config.SMChannel.Name);

            return channelStatus;
        }

        logger.LogInformation("No existing channel for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.SMChannel.Id, config.SMChannel.Name);


        channelStatus = new ChannelStatus(config.SMChannel)
        {
            VideoProfile = VideoOutputProfileDto(config.SMChannel.StreamingProxyType)
        };

        _channelStatuses.TryAdd(config.SMChannel.Id, channelStatus);

        await SetNextChildVideoStream(channelStatus).ConfigureAwait(false);
        await SwitchChannelToNextStream(channelStatus);

        return channelStatus;
    }


    public async Task<IChannelStatus?> SetupChannel(SMChannel smChannel)
    {
        if (smChannel == null)
        {
            throw new ArgumentNullException(nameof(smChannel));
        }

        IChannelStatus? channelStatus = GetChannelStatus(smChannel.Id);
        if (channelStatus == null)
        {
            channelStatus = new ChannelStatus(smChannel);
            channelStatus.VideoProfile = VideoOutputProfileDto(smChannel.StreamingProxyType);
            _channelStatuses.TryAdd(smChannel.Id, channelStatus);

            await SetNextChildVideoStream(channelStatus).ConfigureAwait(false);

        }

        return channelStatus;
    }

    public void UnRegisterChannel(int smChannelId)
    {
        _channelStatuses.TryRemove(smChannelId, out _);
    }

    private List<VideoOutputProfileDto> GetProfiles()
    {

        List<VideoOutputProfileDto> ret = [];

        foreach (string key in intprofilesettings.CurrentValue.VideoProfiles.Keys)
        {
            ret.Add(new VideoOutputProfileDto
            {
                ProfileName = key,
                IsReadOnly = intprofilesettings.CurrentValue.VideoProfiles[key].IsReadOnly,
                Parameters = intprofilesettings.CurrentValue.VideoProfiles[key].Parameters,
                Timeout = intprofilesettings.CurrentValue.VideoProfiles[key].Timeout,
                IsM3U8 = intprofilesettings.CurrentValue.VideoProfiles[key].IsM3U8
            });
        }

        return ret;
    }

    public IChannelStatus? GetChannelStatus(int smChannelId)
    {
        _channelStatuses.TryGetValue(smChannelId, out IChannelStatus? channelStatus);
        return channelStatus;
    }

    public List<IChannelStatus> GetChannelStatusesFromSMStreamId(string StreamId)
    {
        try
        {
            if (StreamId is null)
            {
                logger.LogError("StreamId is null");
                return new List<IChannelStatus>();
            }
            return _channelStatuses.Values.Where(a => a.SMStream.Id == StreamId).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting channel statuses from SMStream ID {StreamId}.", StreamId);
            return new List<IChannelStatus>();
        }
    }

    public List<IChannelStatus> GetChannelStatusesFromSMChannelId(int smChannelId)
    {
        return _channelStatuses.Values.Where(a => a.SMChannel.Id == smChannelId).ToList();
    }

    public List<IChannelStatus> GetChannelStatuses()
    {
        return [.. _channelStatuses.Values];
    }

    public bool HasChannel(int smChannelId)
    {
        return _channelStatuses.ContainsKey(smChannelId);
    }

    public int GetGlobalStreamsCount()
    {
        return _channelStatuses.Values.Count(a => a.IsGlobal);
    }
    public async Task<bool> SwitchChannelToNextStream(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null)
    {
        if (channelStatus.FailoverInProgress)
        {
            return false;
        }

        channelStatus.FailoverInProgress = true;
        if (!string.IsNullOrEmpty(overrideNextVideoStreamId))
        {
            channelStatus.OverrideVideoStreamId = overrideNextVideoStreamId;
        }

        logger.LogDebug("Starting SwitchToNextVideoStream with channelStatus: {channelStatus} and overrideNextVideoStreamId: {overrideNextVideoStreamId}", channelStatus, overrideNextVideoStreamId);

        IStreamHandler? oldStreamHandler = streamManager.GetStreamHandler(channelStatus.SMStream?.Url);

        await SetNextChildVideoStream(channelStatus, overrideNextVideoStreamId);

        if (channelStatus.SMStream is null)
        {
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to smStream being null");
            channelStatus.FailoverInProgress = false;
            return false;
        }

        if (oldStreamHandler != null && oldStreamHandler.SMStream.Id == channelStatus.SMStream.Id)
        {
            logger.LogDebug("Matching ids, stopping original stream");
            //oldStreamHandler.SetFailed();

            //channelStatus.FailoverInProgress = false;
            //oldStreamHandler.Stop();
            return true;
        }
        IStreamHandler? newStreamHandler = await streamManager.GetOrCreateStreamHandler(channelStatus);

        if (newStreamHandler is null)
        {
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to channelStatus. newStreamHandler is null");
            channelStatus.FailoverInProgress = false;
            return false;
        }

        if (channelStatus.SMStream is not null && oldStreamHandler is not null)
        {

            await streamManager.MoveClientStreamers(oldStreamHandler, newStreamHandler);
        }
        else
        {
            var clientConfigs = clientStreamerManager.GetClientStreamerConfigurationsBySMChannelId(channelStatus.SMChannel.Id);
            await streamManager.AddClientsToHandler(clientConfigs, newStreamHandler);
        }

        channelStatus.FailoverInProgress = false;

        logger.LogDebug("Finished SwitchToNextVideoStream");
        return true;
    }


    public async Task SetNextChildVideoStream(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

        var m3uFilesRepo = (await repository.M3UFile.GetM3UFiles().ConfigureAwait(false))
                            .ToDictionary(m => m.Id);

        if (!string.IsNullOrEmpty(overrideNextVideoStreamId))
        {
            var smStream = repository.SMStream.GetSMStream(overrideNextVideoStreamId);
            if (smStream == null)
            {
                return;
            }

            if (!m3uFilesRepo.TryGetValue(smStream.M3UFileId, out var m3uFile))
            {
                if (GetGlobalStreamsCount() >= _settings.GlobalStreamLimit)
                {
                    logger.LogInformation("Max global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), smStream.Url);
                    return;
                }

                channelStatus.SetIsGlobal();
                logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
            }
            else if (m3uFile.MaxStreamCount > 0)
            {
                int allStreamsCount = GetCurrentStreamCountForM3UFile(m3uFile.Id);
                if (allStreamsCount >= m3uFile.MaxStreamCount)
                {
                    logger.LogInformation("Max stream count {AllStreamsCount}/{MaxStreams} reached for stream: {StreamUrl}", allStreamsCount, m3uFile.MaxStreamCount, smStream.Url);
                    return;
                }
            }

            logger.LogDebug("Exiting SetNextChildVideoStream with to Return: {Id} {Name}", smStream.Id, smStream.Name);
            channelStatus.SetCurrentSMStream(smStream);
            return;
        }

        var channel = repository.SMChannel.GetSMChannel(channelStatus.SMChannel.Id);
        if (channel == null)
        {
            logger.LogError("SetNextChildVideoStream could not get videoStreams for id {ParentVideoStreamId}", channelStatus.SMChannel.Id);
            channelStatus.SetCurrentSMStream(null);
            return;
        }

        var smStreams = channel.SMStreams.OrderBy(s => s.Rank).Select(a => a.SMStream).ToList();
        if (!smStreams.Any())
        {
            channelStatus.SetCurrentSMStream(null);
            return;
        }

        channelStatus.Rank = Math.Min(channelStatus.Rank, smStreams.Count - 1);

        for (int i = 0; i < smStreams.Count; i++)
        {
            var toReturn = smStreams[channelStatus.Rank];
            channelStatus.Rank = (channelStatus.Rank + 1) % smStreams.Count;

            if (!m3uFilesRepo.TryGetValue(toReturn.M3UFileId, out var m3uFile))
            {
                if (GetGlobalStreamsCount() >= _settings.GlobalStreamLimit)
                {
                    logger.LogInformation("Max global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), toReturn.Url);
                    continue;
                }

                channelStatus.SetIsGlobal();
                logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
            }
            else if (m3uFile.MaxStreamCount > 0)
            {
                int allStreamsCount = GetCurrentStreamCountForM3UFile(m3uFile.Id);
                if (allStreamsCount >= m3uFile.MaxStreamCount)
                {
                    logger.LogInformation("Max stream count {AllStreamsCount}/{MaxStreams} reached for stream: {StreamUrl}", allStreamsCount, m3uFile.MaxStreamCount, toReturn.Url);
                    continue;
                }
            }

            logger.LogDebug("Exiting SetNextChildVideoStream with to Return: {Id} {Name}", toReturn.Id, toReturn.Name);
            channelStatus.SetCurrentSMStream(toReturn);
            return;
        }

        logger.LogDebug("Exiting SetNextChildVideoStream with null due to no suitable videoStream found");
        channelStatus.SetCurrentSMStream(null);
    }

    // Helper method to get current stream count for a specific M3U file
    private int GetCurrentStreamCountForM3UFile(int m3uFileId)
    {
        return 0;
    }


}
