using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

using System.Collections.Concurrent;
namespace StreamMaster.Streams.Channels;

public sealed class ChannelService : IChannelService, IDisposable
{
    private readonly ILogger<ChannelService> logger;
    private readonly ConcurrentDictionary<int, IChannelStatus> _channelStatuses = new();
    private readonly ISwitchToNextStreamService switchToNextStreamService;
    private readonly IChannelBroadcasterService ChannelDistributorService;
    private readonly IChannelStatusService ChannelStatusService;
    private readonly IVideoInfoService VideoInfoService;

    private readonly object _disposeLock = new();
    private bool _disposed = false;

    public ChannelService(
        ILogger<ChannelService> logger,
        IVideoInfoService VideoInfoService,
        IChannelBroadcasterService channelDistributorService,
        IChannelStatusService ChannelStatusService,
        ISwitchToNextStreamService switchToNextStreamService)
    {
        this.VideoInfoService = VideoInfoService;
        ChannelDistributorService = channelDistributorService;
        this.ChannelStatusService = ChannelStatusService;
        this.switchToNextStreamService = switchToNextStreamService;
        this.logger = logger;

        ChannelDistributorService.OnChannelDirectorStoppedEvent += ChannelDistributorService_OnStoppedEvent;
        ChannelStatusService.OnChannelStatusStoppedEvent += ChannelStatusService_OnChannelStatusStoppedEvent;
    }

    private async Task ChannelStatusService_OnChannelStatusStoppedEvent(object? sender, ChannelStatusStopped e)
    {
        if (sender is IChannelStatus channelStatus)
        {
            logger.LogInformation("Streaming Stopped Event for StreamId: {StreamId} {StreamName}", e.Id, e.Name);
            await CloseChannel(channelStatus).ConfigureAwait(false);
        }
    }

    private async Task ChannelDistributorService_OnStoppedEvent(object? sender, ChannelDirectorStopped e)
    {
        if (sender is IChannelBroadcaster channelDistributor)
        {
            logger.LogInformation("Streaming Stopped Event for StreamId: {StreamId} {StreamName}", e.Id, e.Name);

            List<IChannelStatus> affectedChannelStatuses = _channelStatuses.Values
                .Where(a => a.SMStreamInfo?.Url == e.Id)
                .ToList();

            foreach (IChannelStatus? channelStatus in affectedChannelStatuses)
            {
                if (channelStatus.Shutdown)
                {
                    continue;
                }

                if (channelStatus.FailoverInProgress)
                {
                    continue;
                }

                bool didSwitch = await SwitchChannelToNextStream(channelStatus).ConfigureAwait(false);
                if (!didSwitch)
                {
                    await CloseChannel(channelStatus).ConfigureAwait(false);
                }
            }
        }
    }

    public async Task<IClientConfiguration?> GetClientStreamerConfiguration(string UniqueRequestId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();


        IClientConfiguration? config = _channelStatuses.Values
            .SelectMany(a => a.GetClientStreamerConfigurations())
            .FirstOrDefault(a => a.UniqueRequestId == UniqueRequestId);

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
                    channelStatus.Stop();
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

    public async Task<IChannelStatus?> GetChannelStatusAsync(IClientConfiguration config)
    {
        IChannelStatus? channelStatus = GetChannelStatus(config.SMChannel.Id);

        if (channelStatus == null)
        {
            logger.LogInformation("No existing channel for {ChannelVideoStreamId} {name}", config.SMChannel.Id, config.SMChannel.Name);

            channelStatus = await ChannelStatusService.GetOrCreateChannelStatusAsync(config, CancellationToken.None).ConfigureAwait(false);
            _channelStatuses.TryAdd(config.SMChannel.Id, channelStatus);

            if (!await SwitchChannelToNextStream(channelStatus).ConfigureAwait(false))
            {
                await UnRegisterChannelAsync(channelStatus);
                await CheckForEmptyChannelsAsync().ConfigureAwait(false);
                return null;
            }
        }
        else
        {
            logger.LogInformation("Reuse existing stream handler for {ChannelVideoStreamId} {name}", config.SMChannel.Id, config.SMChannel.Name);
        }

        if (channelStatus.SMStreamInfo == null)
        {

            await UnRegisterChannelAsync(channelStatus);
            await CheckForEmptyChannelsAsync().ConfigureAwait(false);
            return null;
        }

        IChannelBroadcaster? channelDistributor = ChannelDistributorService.GetChannelBroadcaster(channelStatus.SMStreamInfo.Url);
        if (channelDistributor is null || channelDistributor.IsFailed)
        {
            if (!await SwitchChannelToNextStream(channelStatus).ConfigureAwait(false))
            {
                logger.LogError("SwitchChannelToNextStream failed for {UniqueRequestId} {ChannelVideoStreamId} {name}", config.UniqueRequestId, config.SMChannel.Id, config.SMChannel.Name);
                await UnRegisterChannelAsync(channelStatus);
                await CheckForEmptyChannelsAsync().ConfigureAwait(false);
                return null;
            }
        }

        channelStatus.AddClientStreamer(config.UniqueRequestId, config);

        return channelStatus;
    }

    public async Task<IChannelStatus?> SetupChannel(SMChannelDto smChannel)
    {
        // Implementation commented out for brevity
        return null;
    }

    private List<string> RunningUrls => _channelStatuses.Values
        .Where(a => a.ClientCount > 0 && a.SMStreamInfo != null)
        .Select(a => a.SMStreamInfo!.Url)
        .Distinct()
        .ToList();

    public async Task CheckForEmptyChannelsAsync(CancellationToken cancellationToken = default)
    {
        TimeSpan delay = TimeSpan.FromSeconds(5);
        List<Task> tasks = [];

        foreach (IChannelStatus? channelStatus in _channelStatuses.Values.Where(a => a.SMStreamInfo != null))
        {
            if (channelStatus.ClientCount == 0 && !RunningUrls.Contains(channelStatus.SMStreamInfo!.Url))
            {
                tasks.Add(CloseChannel(channelStatus));
            }
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public async Task CloseChannel(IChannelStatus channelStatus)
    {
        channelStatus.Shutdown = true;

        bool closed = channelStatus.SMStreamInfo?.ShutDownDelay > 0
            ? await UnRegisterChannelAfterDelayAsync(channelStatus, TimeSpan.FromSeconds(channelStatus.SMStreamInfo.ShutDownDelay), CancellationToken.None).ConfigureAwait(false)
            : await UnRegisterChannelAsync(channelStatus).ConfigureAwait(false);

        if (closed)
        {
            channelStatus.Stop();
            foreach (IClientConfiguration config in channelStatus.GetClientStreamerConfigurations())
            {
                config.ClientStream?.Flush();
                config.ClientStream?.Dispose();
                await config.Response.CompleteAsync().ConfigureAwait(false);
            }

            List<IChannelBroadcaster> channelBroadcasters = ChannelDistributorService.GetChannelBroadcasters()
                .Where(cd => cd.ClientChannels.Any(cc => cc.Key == channelStatus.Id.ToString()))
                .ToList();

            foreach (IChannelBroadcaster? channelBroadcaster in channelBroadcasters)
            {
                if (channelBroadcaster.RemoveClientChannel(channelStatus.Id))
                {
                    ChannelDistributorService.StopAndUnRegister(channelBroadcaster.Id);
                }
            }

            await CheckForEmptyChannelsAsync().ConfigureAwait(false);
        }
    }

    private async Task<bool> UnRegisterChannelAfterDelayAsync(IChannelStatus channelStatus, TimeSpan delay, CancellationToken cancellationToken)
    {
        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

        return channelStatus.ClientCount == 0 && channelStatus.SMStreamInfo != null && !RunningUrls.Contains(channelStatus.SMStreamInfo.Url)
&& await UnRegisterChannelAsync(channelStatus).ConfigureAwait(false);
    }

    private async Task<bool> UnRegisterChannelAsync(IChannelStatus channelStatus)
    {
        _channelStatuses.TryRemove(channelStatus.SMChannel.Id, out _);
        if (channelStatus.SMStreamInfo?.Url != null)
        {
            ChannelStatusService.StopAndUnRegisterChannelStatus(channelStatus.Id);
        }

        foreach (IClientConfiguration config in channelStatus.GetClientStreamerConfigurations())
        {
            await UnRegisterClient(config.UniqueRequestId).ConfigureAwait(false);
        }

        return true;
    }

    public IChannelStatus? GetChannelStatus(int smChannelId)
    {
        _channelStatuses.TryGetValue(smChannelId, out IChannelStatus? channelStatus);
        return channelStatus;
    }

    public List<IChannelStatus> GetChannelStatusesFromSMStreamId(string StreamId)
    {
        if (StreamId == null)
        {
            logger.LogError("StreamId is null");
            return [];
        }

        try
        {
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
        return _channelStatuses.Values.ToList();
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

        logger.LogDebug("Starting SwitchToNextVideoStream with channelStatus: {channelStatus} and overrideNextVideoStreamId: {overrideNextVideoStreamId}", channelStatus, OverrideSMStreamId);

        bool didChange = await switchToNextStreamService.SetNextStreamAsync(channelStatus, OverrideSMStreamId).ConfigureAwait(false);

        if (channelStatus.SMStreamInfo == null || !didChange)
        {
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to smStream being null");
            channelStatus.FailoverInProgress = false;
            return false;
        }

        IChannelBroadcaster? sourceChannelBroadcaster = await ChannelDistributorService.GetOrCreateChannelDistributorAsync(channelStatus.SMChannel.Name, channelStatus.SMStreamInfo, CancellationToken.None).ConfigureAwait(false);

        if (sourceChannelBroadcaster == null)
        {
            logger.LogDebug("Exiting, Source Channel Distributor is null");
            channelStatus.FailoverInProgress = false;
            return false;
        }

        channelStatus.SetSourceChannelBroadcaster(sourceChannelBroadcaster);

        if (!channelStatus.SMStreamInfo.Id.StartsWith(IntroPlayListBuilder.IntroIDPrefix, StringComparison.InvariantCulture))
        {
            VideoInfoService.SetSourceChannel(sourceChannelBroadcaster);
        }

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

            if (configToRemove != null && channelStatus.RemoveClientStreamer(UniqueRequestId))
            {
                removed = true;
                break;
            }
        }

        if (removed)
        {
            await CheckForEmptyChannelsAsync(cancellationToken).ConfigureAwait(false);
            logger.LogDebug("Client configuration for {UniqueRequestId} removed", UniqueRequestId);
        }
        else
        {
            logger.LogDebug("Client configuration for {UniqueRequestId} not found", UniqueRequestId);
        }

        return await Task.FromResult(removed).ConfigureAwait(false);
    }
}
