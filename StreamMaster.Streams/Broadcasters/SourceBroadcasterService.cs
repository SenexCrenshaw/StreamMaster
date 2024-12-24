using System.Collections.Concurrent;

using StreamMaster.Domain.Events;
using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Broadcasters;

public class SourceBroadcasterService(
    ILogger<SourceBroadcasterService> logger,
    IVideoInfoService videoInfoService,
    ILogger<ISourceBroadcaster> sourceBroadcasterLogger,
    IOptionsMonitor<Setting> settings,
    IStreamFactory streamFactory)
    : ISourceBroadcasterService
{
    public event AsyncEventHandler<StreamBroadcasterStopped>? OnStreamBroadcasterStoppedEvent;

    private readonly ConcurrentDictionary<string, ISourceBroadcaster> sourceBroadcasters = new();
    private readonly SemaphoreSlim getOrCreateStreamDistributorSlim = new(1, 1);

    public ISourceBroadcaster? GetStreamBroadcaster(string? key)
    {
        return string.IsNullOrEmpty(key) ? null : sourceBroadcasters.TryGetValue(key, out ISourceBroadcaster? broadcaster) ? broadcaster : null;
    }

    public List<ISourceBroadcaster> GetStreamBroadcasters()
    {
        return [.. sourceBroadcasters.Values];
    }

    public async Task<ISourceBroadcaster?> GetOrCreateStreamBroadcasterAsync(SMStreamInfo smStreamInfo, CancellationToken cancellationToken)
    {
        return await GetOrCreateSourceBroadcasterInternalAsync(
            smStreamInfo,
            null,
            async (sourceBroadcaster, _, token) => await sourceBroadcaster.SetSourceStreamAsync(smStreamInfo, token).ConfigureAwait(false),
            cancellationToken);
    }

    public async Task<ISourceBroadcaster?> GetOrCreateMultiViewStreamBroadcasterAsync(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken)
    {
        return channelBroadcaster == null || channelBroadcaster.SMStreamInfo is null
            ? null
            : await GetOrCreateSourceBroadcasterInternalAsync(
            channelBroadcaster.SMStreamInfo,
            channelBroadcaster,
            async (sourceBroadcaster, channelBroadcaster, token) => await sourceBroadcaster.SetSourceMultiViewStreamAsync(channelBroadcaster!, token).ConfigureAwait(false),
            cancellationToken);
    }

    private async Task<ISourceBroadcaster?> GetOrCreateSourceBroadcasterInternalAsync(
        SMStreamInfo smStreamInfo,
        IChannelBroadcaster? channelBroadcaster,
        Func<ISourceBroadcaster, IChannelBroadcaster?, CancellationToken, Task> setupBroadcaster,
        CancellationToken cancellationToken)
    {
        if (smStreamInfo == null)
        {
            return null;
        }

        await getOrCreateStreamDistributorSlim.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (sourceBroadcasters.TryGetValue(smStreamInfo.Url, out ISourceBroadcaster? sourceBroadcaster))
            {
                if (sourceBroadcaster.IsFailed)
                {
                    await StopAndUnRegisterSourceBroadcasterAsync(smStreamInfo.Url).ConfigureAwait(false);
                }
                else
                {
                    logger.LogInformation("Reusing source stream: {Id} {Name}", smStreamInfo.Id, smStreamInfo.Name);
                    return sourceBroadcaster;
                }
            }

            sourceBroadcaster = new SourceBroadcaster(sourceBroadcasterLogger, settings, streamFactory, smStreamInfo);
            logger.LogInformation("Created new source stream for: {Id} {Name}", smStreamInfo.Id, smStreamInfo.Name);

            await setupBroadcaster(sourceBroadcaster, channelBroadcaster, cancellationToken).ConfigureAwait(false);
            sourceBroadcaster.StreamBroadcasterStopped += OnStoppedEvent;

            if (sourceBroadcasters.TryAdd(smStreamInfo.Url, sourceBroadcaster))
            {
                if (!smStreamInfo.Id.StartsWith(IntroPlayListBuilder.IntroIDPrefix, StringComparison.InvariantCulture))
                {
                    videoInfoService.SetSourceChannel(sourceBroadcaster, smStreamInfo.Id, smStreamInfo.Name);
                }
            }

            return sourceBroadcaster;
        }
        finally
        {
            getOrCreateStreamDistributorSlim.Release();
        }
    }

    public async Task<bool> StopAndUnRegisterSourceBroadcasterAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        if (sourceBroadcasters.TryRemove(key, out ISourceBroadcaster? sourceBroadcaster))
        {
            try
            {
                await sourceBroadcaster.StopAsync().ConfigureAwait(false);
                videoInfoService.StopVideoPlugin(sourceBroadcaster.Id);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error stopping source broadcaster {Key}", key);
            }
        }

        return false;
    }

    private async void OnStoppedEvent(object? sender, StreamBroadcasterStopped e)
    {
        if (sender is ISourceBroadcaster sourceBroadcaster)
        {
            await StopAndUnRegisterSourceBroadcasterAsync(e.Id).ConfigureAwait(false);

            AsyncEventHandler<StreamBroadcasterStopped>? handler = OnStreamBroadcasterStoppedEvent;
            if (handler != null)
            {
                await handler.Invoke(sender, e).ConfigureAwait(false);
            }
        }
    }

    private async Task CheckForEmptyBroadcastersAsync(CancellationToken cancellationToken = default)
    {
        foreach (ISourceBroadcaster sourceBroadcaster in sourceBroadcasters.Values)
        {
            int count = sourceBroadcaster.ChannelBroadcasters.Count(a => a.Key != "VideoInfo");
            if (count == 0)
            {
                int delay = settings.CurrentValue.ShutDownDelay;
                if (delay > 0)
                {
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }

                count = sourceBroadcaster.ChannelBroadcasters.Count(a => a.Key != "VideoInfo");
                if (count == 0)
                {
                    await StopAndUnRegisterSourceBroadcasterAsync(sourceBroadcaster.Id).ConfigureAwait(false);
                }
            }
        }
    }

    public async Task UnRegisterChannelBroadcasterAsync(int channelBroadcasterId)
    {
        ISourceBroadcaster? sourceBroadcaster = sourceBroadcasters.Values
            .FirstOrDefault(broadcaster => broadcaster.ChannelBroadcasters.ContainsKey(channelBroadcasterId.ToString()));

        if (sourceBroadcaster?.RemoveChannelBroadcaster(channelBroadcasterId) == true)
        {
            await CheckForEmptyBroadcastersAsync().ConfigureAwait(false);
        }
    }
}
