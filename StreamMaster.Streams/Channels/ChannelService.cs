using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

using System.Collections.Concurrent;
namespace StreamMaster.Streams.Channels;

public sealed class ChannelService : IChannelService, IDisposable
{
    private readonly ILogger<ChannelService> logger;
    private readonly ILogger<IChannelStatus> channelStatusLogger;
    private readonly ConcurrentDictionary<int, IChannelStatus> _channelStatuses = new();
    private readonly ICustomPlayListBuilder customPlayListBuilder;
    private readonly IOptionsMonitor<Setting> intSettings;
    private readonly IClientStatisticsManager statisticsManager;
    private readonly IProfileService profileService;
    private readonly ISwitchToNextStreamService switchToNextStreamService;
    private readonly IDubcer dubcer;
    private readonly IChannelDistributorService channelDistributorService;
    private readonly IChannelStatusService channelStatusService;
    private readonly IVideoCombinerService videoCombinerService;

    private readonly object _disposeLock = new();
    private bool _disposed = false;

    public ChannelService(
        ILogger<ChannelService> logger,
        IDubcer dubcer,
        IChannelDistributorService channelDistributorService,
        ILogger<IChannelStatus> channelStatusLogger,
        IProfileService profileService,
        ICustomPlayListBuilder customPlayListBuilder,
        IOptionsMonitor<Setting> intSettings,
        ISwitchToNextStreamService switchToNextStreamService,
        IChannelStatusService channelStatusService,
        IVideoCombinerService videoCombinerService,
         IClientStatisticsManager statisticsManager
    )
    {
        this.channelDistributorService = channelDistributorService;
        this.profileService = profileService;
        this.switchToNextStreamService = switchToNextStreamService;
        this.customPlayListBuilder = customPlayListBuilder;
        this.intSettings = intSettings;
        this.channelStatusLogger = channelStatusLogger;
        this.statisticsManager = statisticsManager;
        this.dubcer = dubcer;
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.channelDistributorService.OnStoppedEvent += ChannelDistributorService_OnStoppedEvent;
        this.channelStatusService = channelStatusService;
    }

    private async Task ChannelDistributorService_OnStoppedEvent(object? sender, ChannelDirectorStopped e)
    {
        if (sender is not null and IChannelDistributor channelDistributor)
        {

            logger.LogInformation("Streaming Stopped Event for StreamId: {StreamId} {StreamName}", e.SMStreamInfo.Id, e.SMStreamInfo.Name);
            string url = e.SMStreamInfo.Url;

            List<IChannelStatus> affectedChannelStatuses = _channelStatuses.Values.Where(a => a.SMStreamInfo?.Url == url).ToList();

            foreach (IChannelStatus channelStatus in affectedChannelStatuses)
            {
                if (channelStatus?.Shutdown == false)
                {
                    if (channelStatus.FailoverInProgress)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(channelStatus.OverrideSMStreamId))
                    {
                        channelStatus.OverrideSMStreamId = "";
                        continue;
                    }

                    bool didSwitch = await SwitchChannelToNextStream(channelStatus);

                    if (!didSwitch)
                    {
                        await CloseChannel(channelStatus);

                        continue;
                    }
                }
            }
        }
    }

    public async Task<IClientConfiguration?> GetClientStreamerConfiguration(string UniqueRequestId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IClientConfiguration? config = _channelStatuses.Values.SelectMany(a => a.GetClientStreamerConfigurations()).FirstOrDefault(a => a.UniqueRequestId == UniqueRequestId);
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
                foreach (IChannelStatus channelStatus in _channelStatuses.Values)
                {
                    channelStatus.ChannelDistributor.Stop();
                }
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

    public async Task<IChannelStatus?> RegisterChannel(IClientConfiguration config)
    {
        IChannelStatus? channelStatus = GetChannelStatus(config.SMChannel.Id);

        if (channelStatus == null)
        {
            logger.LogInformation("No existing channel for {UniqueRequestId} {ChannelVideoStreamId} {name}",
                              config.UniqueRequestId, config.SMChannel.Id, config.SMChannel.Name);

            CommandProfileDto commandProfileData = profileService.GetCommandProfile();
            if (commandProfileData == null)
            {
                logger.LogError("Could not find command profile");
                return null;
            }

            channelStatus = channelStatusService.NewChannelStatus(config.SMChannel);
            channelStatus.StreamGroupProfileId = config.StreamGroupProfileId;
            channelStatus.CommandProfile = commandProfileData;


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

        if (channelStatus.SMStreamInfo == null)
        {
            if (_channelStatuses.TryRemove(config.SMChannel.Id, out IChannelStatus? test))
            {
                test.ChannelDistributor.Stop();
            }

            return null;
        }

        IChannelDistributor? channelDistributor = channelDistributorService.GetStreamHandler(channelStatus.SMStreamInfo.Url);
        if (channelDistributor is null)
        {
            logger.LogError("Could not find handler for {UniqueRequestId} {ChannelVideoStreamId} {name}", config.UniqueRequestId, config.SMChannel.Id, config.SMChannel.Name);
            await CheckForEmptyChannelsAsync();
            return null;
        }

        if (channelDistributor.IsFailed)
        {
            logger.LogInformation("Existing handler is failed, creating");
            if (!await SwitchChannelToNextStream(channelStatus).ConfigureAwait(false))
            {
                logger.LogError("Could SwitchChannelToNextStream failed for {UniqueRequestId} {ChannelVideoStreamId} {name}", config.UniqueRequestId, config.SMChannel.Id, config.SMChannel.Name);
                await CheckForEmptyChannelsAsync();
                return null;
            }
        }
        channelStatus.ClientUserAgent = config.ClientUserAgent;

        channelStatus.AddClient(config.UniqueRequestId, config);

        return channelStatus;
    }

    public async Task<IChannelStatus?> SetupChannel(SMChannelDto smChannel)
    {
        ArgumentNullException.ThrowIfNull(smChannel);

        IChannelStatus? channelStatus = GetChannelStatus(smChannel.Id);
        if (channelStatus == null)
        {
            channelStatus = channelStatusService.NewChannelStatus(smChannel);
            channelStatus.CommandProfile = new CommandProfileDto();

            if (!await switchToNextStreamService.SetNextStreamAsync(channelStatus))
            {
                return null;
            }

            Setting settings = intSettings.CurrentValue;
            channelStatus.ClientUserAgent = settings.ClientUserAgent;
            _ = _channelStatuses.TryAdd(smChannel.Id, channelStatus);
        }

        return channelStatus;
    }

    private List<string> RunningUrls => _channelStatuses.Values.Where(a => a.ClientCount > 0 && a.SMStreamInfo != null).Select(a => a.SMStreamInfo!.Url).Distinct().ToList();

    public async Task CheckForEmptyChannelsAsync(CancellationToken cancellationToken = default)
    {
        TimeSpan delay = TimeSpan.FromSeconds(5);
        List<Task> tasks = [];

        foreach (IChannelStatus channelStatus in _channelStatuses.Values.Where(a => a.SMStreamInfo != null))
        {
            if (channelStatus.ClientCount == 0 && !RunningUrls.Contains(channelStatus.SMStreamInfo!.Url))
            {
                await CloseChannel(channelStatus);
            }
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public async Task CloseChannel(IChannelStatus channelStatus)
    {
        bool closed = true;

        if (channelStatus.SMStreamInfo?.IsCustomStream == true)
        {
            await UnRegisterChannelAsync(channelStatus);
        }
        else
        {
            TimeSpan delay = TimeSpan.FromSeconds(5);
            closed = await UnRegisterChannelAfterDelayAsync(channelStatus, delay, CancellationToken.None);
        }
        if (closed)
        {
            channelStatus.ChannelDistributor.Stop();
            foreach (ClientConfiguration config in channelStatus.GetClientStreamerConfigurations())
            {
                config.ClientStream?.Flush();
                config.ClientStream?.Dispose();
                await config.Response.CompleteAsync();
            }
        }
    }

    private async Task<bool> UnRegisterChannelAfterDelayAsync(IChannelStatus channelStatus, TimeSpan delay, CancellationToken cancellationToken)
    {
        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

        if (channelStatus.ClientCount == 0)
        {
            if (channelStatus.SMStreamInfo != null && !RunningUrls.Contains(channelStatus.SMStreamInfo.Url))
            {
                await UnRegisterChannelAsync(channelStatus);
                return true;
            }
        }

        return false;
    }

    private async Task UnRegisterChannelAsync(IChannelStatus channelStatus)
    {
        _channelStatuses.TryRemove(channelStatus.SMChannel.Id, out _);
        if (channelStatus.SMStreamInfo?.Url != null)
        {
            channelDistributorService.StopAndUnRegister(channelStatus.SMStreamInfo.Url);
        }

        foreach (IClientConfiguration config in channelStatus.GetClientStreamerConfigurations())
        {
            await UnRegisterClient(config.UniqueRequestId);
        }
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
            return _channelStatuses.Values.Where(a => a?.SMStreamInfo?.Id == StreamId).ToList();
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
        return _channelStatuses.Values.Where(a => a?.SMStreamInfo?.Url == videoUrl).ToList();
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
    public async Task<bool> SwitchChannelToNextStream(IChannelStatus channelStatus, string? OverrideSMStreamId = null)
    {
        if (channelStatus.FailoverInProgress)
        {
            return false;
        }


        channelStatus.OverrideSMStreamId = OverrideSMStreamId;


        logger.LogDebug("Starting SwitchToNextVideoStream with channelStatus: {channelStatus} and overrideNextVideoStreamId: {overrideNextVideoStreamId}", channelStatus, OverrideSMStreamId);

        bool didChange = await switchToNextStreamService.SetNextStreamAsync(channelStatus, OverrideSMStreamId);

        if (channelStatus.SMStreamInfo is null || !didChange)
        {
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to smStream being null");
            channelStatus.FailoverInProgress = false;
            return false;
        }

        IChannelDistributor? sourceChannelDistributor = await channelDistributorService.GetOrCreateSourceChannelDistributorAsync(channelStatus.SMStreamInfo, CancellationToken.None);

        if (sourceChannelDistributor is null)
        {
            logger.LogDebug("Exiting, Source Channel Distributor is null");
            channelStatus.FailoverInProgress = false;
            return false;
        }

        channelStatus.SetSourceChannel(sourceChannelDistributor, channelStatus.SMStreamInfo.Name, channelStatus.SMStreamInfo.IsCustomStream);

        channelStatus.FailoverInProgress = false;

        logger.LogDebug("Finished SwitchToNextVideoStream");
        return true;
    }

    public async Task<bool> UnRegisterClient(string UniqueRequestId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        bool removed = false;
        foreach (IChannelStatus channelStatus in _channelStatuses.Values)
        {
            List<IClientConfiguration> clientConfigs = channelStatus.GetClientStreamerConfigurations();
            IClientConfiguration? configToRemove = clientConfigs.Find(a => a.UniqueRequestId == UniqueRequestId);

            if (channelStatus.RemoveClient(UniqueRequestId))
            {
                removed = true;
                break;
            }
        }

        if (removed)
        {
            await CheckForEmptyChannelsAsync(cancellationToken);
            logger.LogDebug("Client configuration for {UniqueRequestId} removed", UniqueRequestId);
        }
        else
        {
            logger.LogDebug("Client configuration for {UniqueRequestId} not found", UniqueRequestId);
        }

        return await Task.FromResult(removed).ConfigureAwait(false);
    }
}
