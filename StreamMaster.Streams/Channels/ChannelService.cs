using AutoMapper;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Application.Profiles.Queries;
using StreamMaster.Domain.API;
using StreamMaster.Domain.Configuration;
using StreamMaster.PlayList;
using StreamMaster.Streams.Buffers;

using System.Collections.Concurrent;
namespace StreamMaster.Streams.Channels;

public sealed class ChannelService : IChannelService, IDisposable

{
    private readonly ILogger<ChannelService> logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly ConcurrentDictionary<int, IChannelStatus> _channelStatuses = new();
    private readonly object _disposeLock = new();
    private readonly ILoggerFactory loggerFactory;
    private readonly IStreamManager streamManager;
    private readonly IMapper mapper;
    private readonly ICustomPlayListBuilder customPlayListBuilder;
    private readonly IOptionsMonitor<Setting> settingsMonitor;
    private readonly IClientStatisticsManager statisticsManager;

    private bool _disposed = false;

    public ChannelService(
        ILogger<ChannelService> logger,
        ILoggerFactory loggerFactory,
        IStreamManager streamManager,
        IServiceProvider serviceProvider,
        IMapper mapper,
        ICustomPlayListBuilder customPlayListBuilder,
        IOptionsMonitor<Setting> settingsMonitor,
         IClientStatisticsManager statisticsManager
    )
    {
        this.loggerFactory = loggerFactory;
        this.streamManager = streamManager;
        this.mapper = mapper;
        this.customPlayListBuilder = customPlayListBuilder;
        this.settingsMonitor = settingsMonitor;
        this.statisticsManager = statisticsManager;
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.streamManager.OnStreamingStoppedEvent += StreamManager_OnStreamingStoppedEvent;
    }

    private async void StreamManager_OnStreamingStoppedEvent(object? sender, StreamHandlerStopped StoppedEvent)
    {
        if (sender is not null and IStreamHandler streamHandler)
        {
            logger.LogInformation("Streaming Stopped Event for StreamId: {StreamId} {StreamName}", streamHandler.SMStream.Id, streamHandler.SMStream.Name);

            //List<IChannelStatus> affectedChannelStatuses = streamHandler.GetChannelStatuses.ToList();

            //foreach (IChannelStatus channelStatus in affectedChannelStatuses)
            //{
            //    if (channelStatus != null && channelStatus.Shutdown != true)
            //    {
            //        if (channelStatus.FailoverInProgress)
            //        {
            //            continue;
            //        }

            //        if (!string.IsNullOrEmpty(channelStatus.OverrideVideoStreamId))
            //        {
            //            channelStatus.OverrideVideoStreamId = "";
            //            continue;
            //        }

            //        bool didSwitch = await SwitchChannelToNextStream(channelStatus);

            //        if (!didSwitch)
            //        {
            //            //clientStreamerManager.GetClientStreamerConfigurationsBySMChannelId(channelStatus.SMChannel.Id)
            //            //    .ForEach(async x =>
            //            //    {
            //            //        await CancelClient(x.UniqueRequestId);
            //            //    }
            //            //    );

            //            continue;
            //        }
            //    }
            //}

            //if (streamHandler.ChannelCount == 0)
            //{
            //    _ = streamManager.StopAndUnRegisterHandler(streamHandler.SMStream.Url);

        }
    }
    public async Task<ClientStreamerConfiguration?> GetClientStreamerConfiguration(string UniqueRequestId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ClientStreamerConfiguration? config = _channelStatuses.Values.SelectMany(a => a.ClientStreamerConfigurations.Values).FirstOrDefault(a => a.UniqueRequestId == UniqueRequestId);
        if (config != null)
        {
            return await Task.FromResult(config).ConfigureAwait(false);
        }
        logger.LogDebug("Client configuration for {UniqueRequestId} not found", UniqueRequestId);
        return null;
    }

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

    public async Task<IChannelStatus?> RegisterChannel(ClientStreamerConfiguration config)
    {
        config.ClientStream ??= new ClientReadStream(statisticsManager, loggerFactory, config);

        IChannelStatus? channelStatus = GetChannelStatus(config.SMChannel.Id);

        if (channelStatus == null)
        {
            logger.LogInformation("No existing channel for {UniqueRequestId} {ChannelVideoStreamId} {name}",
                              config.UniqueRequestId, config.SMChannel.Id, config.SMChannel.Name);

            using IServiceScope scope = _serviceProvider.CreateScope();
            ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();

            DataResponse<CommandProfileDto> commandProfileData = await sender.Send(new GetCommandProfileRequest(BuildInfo.DefaultCommandProfileName,
                                                                                   config.StreamGroupId,
                                                                                   config.StreamGroupProfileId)).ConfigureAwait(false);
            if (commandProfileData.Data == null)
            {
                logger.LogError("Could not find video profile for {CommandProfileName}", config.SMChannel.CommandProfileName);
                return null;
            }

            channelStatus = new ChannelStatus(config.SMChannel)
            {
                StreamGroupProfileId = config.StreamGroupProfileId,
                CommandProfile = commandProfileData.Data
            };

            if (config.SMChannel.IsCustomStream)
            {
                channelStatus.CustomPlayList = customPlayListBuilder.GetCustomPlayList(config.SMChannel.Name);
            }

            _channelStatuses.TryAdd(config.SMChannel.Id, channelStatus);

            if (!await SwitchChannelToNextStream(channelStatus).ConfigureAwait(false))
            {
                await CheckForEmptyChannelsAsync();
                return null;
            }
        }
        else
        {
            logger.LogInformation("Reuse existing stream handler for {UniqueRequestId} {ChannelVideoStreamId} {name}",
                                  config.UniqueRequestId, config.SMChannel.Id, config.SMChannel.Name);
        }

        IStreamHandler? handler = streamManager.GetStreamHandler(channelStatus.SMStream.Url);
        if (handler is null)
        {
            logger.LogError("Could not find handler for {UniqueRequestId} {ChannelVideoStreamId} {name}", config.UniqueRequestId, config.SMChannel.Id, config.SMChannel.Name);
            //await clientStreamerManager.UnRegisterClient(config.UniqueRequestId).ConfigureAwait(false);
            await CheckForEmptyChannelsAsync();
            return null;
        }

        if (handler.IsFailed)
        {
            logger.LogInformation("Existing handler is failed, creating");
            if (!await SwitchChannelToNextStream(channelStatus).ConfigureAwait(false))
            {
                logger.LogError("Could SwitchChannelToNextStream failed for {UniqueRequestId} {ChannelVideoStreamId} {name}", config.UniqueRequestId, config.SMChannel.Id, config.SMChannel.Name);
                //await clientStreamerManager.UnRegisterClient(config.UniqueRequestId).ConfigureAwait(false);
                await CheckForEmptyChannelsAsync();
                return null;
            }
        }

        channelStatus.ClientStreamerConfigurations.TryAdd(config.UniqueRequestId, config);

        return channelStatus;
    }

    public async Task<IChannelStatus?> SetupChannel(SMChannelDto smChannel)
    {
        if (smChannel == null)
        {
            throw new ArgumentNullException(nameof(smChannel));
        }

        IChannelStatus? channelStatus = GetChannelStatus(smChannel.Id);
        if (channelStatus == null)
        {
            channelStatus = new ChannelStatus(smChannel)
            {
                CommandProfile = new CommandProfileDto()
            };
            _ = _channelStatuses.TryAdd(smChannel.Id, channelStatus);

            _ = await SetNextChildVideoStream(channelStatus).ConfigureAwait(false);
        }

        return channelStatus;
    }

    private List<string> RunningUrls => _channelStatuses.Values.Where(a => a.ClientCount > 0).Select(a => a.SMStream.Url).Distinct().ToList();

    public async Task CheckForEmptyChannelsAsync(CancellationToken cancellationToken = default)
    {
        TimeSpan delay = TimeSpan.FromSeconds(5);
        List<Task> tasks = [];

        foreach (IChannelStatus channelStatus in _channelStatuses.Values)
        {
            if (channelStatus.ClientCount == 0 && !RunningUrls.Contains(channelStatus.SMStream.Url))
            {
                tasks.Add(UnregisterChannelAfterDelayAsync(channelStatus, delay, cancellationToken));
            }
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private async Task UnregisterChannelAfterDelayAsync(IChannelStatus channelStatus, TimeSpan delay, CancellationToken cancellationToken)
    {
        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

        if (channelStatus.ClientCount == 0 && !RunningUrls.Contains(channelStatus.SMStream.Url))
        {
            UnRegisterChannel(channelStatus);
        }
    }

    private void UnRegisterChannel(IChannelStatus channelStatus)
    {
        _channelStatuses.TryRemove(channelStatus.SMChannel.Id, out _);
        streamManager.StopAndUnRegisterHandler(channelStatus.SMStream.Url);
    }

    public IChannelStatus? GetChannelStatus(int smChannelId)
    {
        _ = _channelStatuses.TryGetValue(smChannelId, out IChannelStatus? channelStatus);
        return channelStatus;
    }

    public List<IChannelStatus> GetChannelStatusesFromSMStreamId(string StreamId)
    {
        try
        {
            if (StreamId is null)
            {
                logger.LogError("StreamId is null");
                return [];
            }
            return _channelStatuses.Values.Where(a => a.SMStream.Id == StreamId).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting channel statuses from SMStream ID {StreamId}.", StreamId);
            return [];
        }
    }

    public IChannelStatus? GetChannelStatusFromSMChannelId(int smChannelId)
    {
        return _channelStatuses.TryGetValue(smChannelId, out IChannelStatus? channelStatus) ? channelStatus : null;
    }

    public List<IChannelStatus> GetChannelStatusFromStreamUrl(string videoUrl)
    {
        return _channelStatuses.Values.Where(a => a.SMStream.Url == videoUrl).ToList();
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
        //_ = streamManager.GetStreamHandler(channelStatus.SMStream?.Url);

        bool didChange = await SetNextChildVideoStream(channelStatus, overrideNextVideoStreamId);

        if (channelStatus.SMStream is null || !didChange)
        {
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to smStream being null");
            channelStatus.FailoverInProgress = false;
            return false;
        }

        IStreamHandler? newStreamHandler = await streamManager.GetOrCreateStreamHandler(channelStatus);

        if (newStreamHandler is null)
        {
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to channelStatus. newStreamHandler is null");
            channelStatus.FailoverInProgress = false;

            return false;
        }

        channelStatus.SetSourceChannel(newStreamHandler.GetOutputChannelReader(), CancellationToken.None);
        //newStreamHandler.RegisterChannel(channelStatus);

        //List<ClientStreamerConfiguration> clientConfigs = clientStreamerManager.GetClientStreamerConfigurationsBySMChannelId(channelStatus.SMChannel.Id);
        //streamManager.AddClientsToHandler(channelStatus, newStreamHandler);

        channelStatus.FailoverInProgress = false;

        logger.LogDebug("Finished SwitchToNextVideoStream");
        return true;
    }

    public async Task<bool> SetNextChildVideoStream(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null)
    {
        if (channelStatus.SMChannel.IsCustomStream)
        {
            if (channelStatus.CustomPlayList == null)
            {
                return false;
            }

            if (channelStatus.CurrentRank > -1)
            {
                channelStatus.CurrentRank++;

                if (channelStatus.CurrentRank >= channelStatus.CustomPlayList.CustomStreamNfos.Count)
                {
                    channelStatus.CurrentRank = 0;
                }
                return true;
            }
        }

        Setting _settings = settingsMonitor.CurrentValue ?? throw new ArgumentNullException(nameof(settingsMonitor));
        using IServiceScope scope = _serviceProvider.CreateScope();
        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

        Dictionary<int, M3UFileDto> m3uFilesRepo = (await repository.M3UFile.GetM3UFiles().ConfigureAwait(false))
                            .ToDictionary(m => m.Id);

        if (!string.IsNullOrEmpty(overrideNextVideoStreamId))
        {
            SMStreamDto? smStream = repository.SMStream.GetSMStream(overrideNextVideoStreamId);
            if (smStream == null)
            {
                return false;
            }

            if (!m3uFilesRepo.TryGetValue(smStream.M3UFileId, out M3UFileDto? m3uFile))
            {
                if (GetGlobalStreamsCount() >= _settings.GlobalStreamLimit)
                {
                    logger.LogInformation("Max global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), smStream.Url);
                    return false;
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
                    return false;
                }
            }

            logger.LogDebug("Exiting SetNextChildVideoStream with to Return: {Id} {Name}", smStream.Id, smStream.Name);
            channelStatus.SetCurrentSMStream(smStream);
            return false;
        }

        SMChannel? channel = repository.SMChannel.GetSMChannel(channelStatus.SMChannel.Id);
        if (channel == null || channel.SMStreams.Count == 0)
        {
            logger.LogError("SetNextChildVideoStream could not get videoStreams for id {ParentVideoStreamId}", channelStatus.SMChannel.Id);
            channelStatus.SetCurrentSMStream(null);
            return false;
        }

        if (channelStatus.CurrentRank + 1 >= channel.SMStreams.Count)
        {
            logger.LogInformation("SetNextChildVideoStream no more streams for id {ParentVideoStreamId}, exiting", channelStatus.SMChannel.Id);
            channelStatus.SetCurrentSMStream(null);
            return false;
        }

        List<SMStream> smStreams = channel.SMStreams.OrderBy(a => a.Rank).Select(a => a.SMStream).ToList();

        for (int i = 0; i < smStreams.Count; i++)
        {
            channelStatus.CurrentRank = (channelStatus.CurrentRank + 1) % smStreams.Count;
            SMStream toReturn = smStreams[channelStatus.CurrentRank];

            if (!m3uFilesRepo.TryGetValue(toReturn.M3UFileId, out M3UFileDto? m3uFile))
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
            SMStreamDto a = mapper.Map<SMStreamDto>(toReturn);
            channelStatus.SetCurrentSMStream(a);
            return true;
        }
        return false;
    }

    // Helper method to get current stream count for a specific M3U file
    private int GetCurrentStreamCountForM3UFile(int m3uFileId)
    {
        return 0;
    }

    public async Task<bool> UnRegisterClient(string UniqueRequestId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        bool removed = false;
        foreach (IChannelStatus channelStatus in _channelStatuses.Values)
        {
            ConcurrentDictionary<string, ClientStreamerConfiguration> clientConfigs = channelStatus.ClientStreamerConfigurations;
            ClientStreamerConfiguration? configToRemove = clientConfigs.Values.FirstOrDefault(a => a.UniqueRequestId == UniqueRequestId);

            if (configToRemove != null)
            {
                if (clientConfigs.TryRemove(configToRemove.UniqueRequestId, out ClientStreamerConfiguration? config))
                {
                    await CheckForEmptyChannelsAsync(cancellationToken);
                    removed = true;
                    break;
                }
            }
        }

        if (removed)
        {
            logger.LogDebug("Client configuration for {UniqueRequestId} removed", UniqueRequestId);
        }
        else
        {
            logger.LogDebug("Client configuration for {UniqueRequestId} not found", UniqueRequestId);
        }

        return await Task.FromResult(removed).ConfigureAwait(false);
    }
}
