﻿using StreamMaster.Domain.Events;
using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Services;

namespace StreamMaster.Streams.Broadcasters;

/// <summary>
/// Service for managing channel broadcasters and their clients.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ChannelBroadcasterService"/> class.
/// </remarks>
public class ChannelBroadcasterService(
    ILogger<ChannelBroadcasterService> logger,
    ICacheManager cacheManager,
    IOptionsMonitor<Setting> settings,
    IStreamLimitsService streamLimitsService,
    ILogger<IChannelBroadcaster> channelStatusLogger
    ) : IChannelBroadcasterService, IDisposable
{
    //private readonly ConcurrentDictionary<int, SemaphoreSlim> channelSemaphores = new();

    private readonly ChannelLockService channelLockService = new();
    private bool disposed;

    /// <inheritdoc/>
    public event AsyncEventHandler<ChannelBroadcasterStopped>? OnChannelBroadcasterStoppedEvent;

    /// <inheritdoc/>
    public async Task<IChannelBroadcaster> GetOrCreateChannelBroadcasterAsync(IClientConfiguration config, int streamGroupProfileId, CancellationToken cancellationToken)
    {
        int channelId = config.SMChannel.Id;
        await channelLockService.AcquireLockAsync(channelId).ConfigureAwait(false);

        try
        {
            if (cacheManager.ChannelBroadcasters.TryGetValue(config.SMChannel.Id, out IChannelBroadcaster? channelBroadcaster))
            {
                if (channelBroadcaster.IsFailed)
                {
                    await UnRegisterChannelAsync(config.SMChannel.Id).ConfigureAwait(false);
                }
                else
                {
                    logger.LogInformation("Reusing channel broadcaster: {Id} {Name}", config.SMChannel.Id, config.SMChannel.Name);
                    return channelBroadcaster;
                }
            }

            channelBroadcaster = new ChannelBroadcaster(channelStatusLogger, config.SMChannel, streamGroupProfileId);

            logger.LogInformation("Created new channel for: {Id} {Name}", config.SMChannel.Id, config.SMChannel.Name);

            channelBroadcaster.OnChannelBroadcasterStoppedEvent += async (sender, args) =>
                await OnStoppedEvent(sender, args).ConfigureAwait(false);

            cacheManager.ChannelBroadcasters.TryAdd(config.SMChannel.Id, channelBroadcaster);

            return channelBroadcaster;
        }
        finally
        {
            channelLockService.ReleaseLock(channelId);
        }
    }

    /// <inheritdoc/>
    public List<IChannelBroadcaster> GetChannelBroadcasters()
    {
        return [.. cacheManager.ChannelBroadcasters.Values];
    }

    /// <inheritdoc/>
    public async Task StopChannelAsync(IChannelBroadcaster channelBroadcaster)
    {
        await StopChannelAsync(channelBroadcaster.Id).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task StopChannelAsync(int channelBroadcasterId)
    {
        if (!cacheManager.ChannelBroadcasters.TryGetValue(channelBroadcasterId, out IChannelBroadcaster? channelBroadcaster))
        {
            return;
        }

        foreach (IClientConfiguration client in channelBroadcaster.Clients.Values)
        {
            await UnRegisterClientAsync(client.UniqueRequestId).ConfigureAwait(false);
        }

        channelBroadcaster.Shutdown = true;

        int delay = channelBroadcaster.SMStreamInfo != null && !channelBroadcaster.ClientConfigurationsEmpty && !streamLimitsService.IsLimited(channelBroadcaster.SMStreamInfo.Id)
            ? settings.CurrentValue.ShutDownDelay
            : 0;

        if (delay > 0)
        {
            await UnRegisterChannelAfterDelayAsync(channelBroadcaster, TimeSpan.FromMilliseconds(delay), CancellationToken.None).ConfigureAwait(false);
        }
        else
        {
            await UnRegisterChannelAsync(channelBroadcaster.SMChannel.Id).ConfigureAwait(false);
        }
    }

    private async Task<bool> UnRegisterChannelAfterDelayAsync(IChannelBroadcaster channelBroadcaster, TimeSpan delay, CancellationToken cancellationToken)
    {
        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        return channelBroadcaster.Clients.IsEmpty && await UnRegisterChannelAsync(channelBroadcaster.SMChannel.Id).ConfigureAwait(false);
    }

    private async Task<bool> UnRegisterChannelAsync(int channelBroadcasterId)
    {
        if (cacheManager.ChannelBroadcasters.TryRemove(channelBroadcasterId, out IChannelBroadcaster? channelBroadcaster))
        {
            foreach (IClientConfiguration client in channelBroadcaster.Clients.Values)
            {
                await UnRegisterClientAsync(client.UniqueRequestId).ConfigureAwait(false);
                client.Stop();
            }

            channelBroadcaster.Stop();
            channelLockService.RemoveLock(channelBroadcasterId);
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public async Task UnRegisterClientAsync(string uniqueRequestId, CancellationToken cancellationToken = default)
    {
        foreach (IChannelBroadcaster channelBroadcaster in cacheManager.ChannelBroadcasters.Values)
        {
            if (channelBroadcaster.RemoveClient(uniqueRequestId))
            {
                logger.LogDebug("Client configuration for {UniqueRequestId} removed", uniqueRequestId);
            }

            if (channelBroadcaster.ClientConfigurationsEmpty)
            {
                await StopChannelAsync(channelBroadcaster).ConfigureAwait(false);
            }
        }
    }

    private async Task OnStoppedEvent(object? sender, ChannelBroadcasterStopped e)
    {
        if (sender is IChannelBroadcaster channelBroadcaster)
        {
            channelBroadcaster.Shutdown = true;
            await StopChannelAsync(e.Id).ConfigureAwait(false);
            OnChannelBroadcasterStoppedEvent?.Invoke(sender, e);
        }
    }

    /// <summary>
    /// Disposes the service and releases all resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the service and releases unmanaged and managed resources.
    /// </summary>
    /// <param name="disposing">Indicates whether the method is called from <see cref="Dispose()"/> or finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            disposed = true;
        }
    }
}