﻿using System.Collections.Concurrent;

using StreamMaster.Domain.Events;
using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Domain.Metrics;
using StreamMaster.Streams.Services;

namespace StreamMaster.Streams.Broadcasters
{
    /// <summary>
    /// Provides methods to create, retrieve, and manage source broadcasters.
    /// </summary>
    public class SourceBroadcasterService(
        ILogger<SourceBroadcasterService> logger,
        IVideoInfoService videoInfoService,
        ILogger<ISourceBroadcaster> sourceBroadcasterLogger,
        IOptionsMonitor<Setting> settings,
        IStreamConnectionService streamConnectionService,
        IStreamFactory streamFactory)
        : ISourceBroadcasterService
    {
        private readonly ChannelLockService<string> channelLockService = new();
        private readonly ConcurrentDictionary<string, ISourceBroadcaster> sourceBroadcasters = new();

        /// <inheritdoc />
        public event AsyncEventHandler<SourceBroadcasterStopped>? OnStreamBroadcasterStopped;

        /// <inheritdoc />
        public ISourceBroadcaster? GetStreamBroadcaster(string? key)
        {
            return string.IsNullOrEmpty(key)
                ? null
                : sourceBroadcasters.TryGetValue(key, out ISourceBroadcaster? broadcaster)
                ? broadcaster
                : null;
        }

        /// <inheritdoc />
        public List<ISourceBroadcaster> GetStreamBroadcasters()
        {
            return [.. sourceBroadcasters.Values];
        }

        /// <inheritdoc />
        public async Task<ISourceBroadcaster?> GetOrCreateStreamBroadcasterAsync(
            SMStreamInfo smStreamInfo,
            CancellationToken cancellationToken)
        {
            return await GetOrCreateSourceBroadcasterInternalAsync(
                smStreamInfo,
                channelBroadcaster: null,
                async (sourceBroadcaster, _, token) => await sourceBroadcaster
                    .SetSourceStreamAsync(smStreamInfo, token)
                    .ConfigureAwait(false),
                true,
                cancellationToken
            ).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ISourceBroadcaster?> GetOrCreateMultiViewStreamBroadcasterAsync(
            IChannelBroadcaster channelBroadcaster,
            CancellationToken cancellationToken)
        {
            return channelBroadcaster == null || channelBroadcaster.SMStreamInfo is null
                ? null
                : await GetOrCreateSourceBroadcasterInternalAsync(
                channelBroadcaster.SMStreamInfo,
                channelBroadcaster,
                async (sourceBroadcaster, cb, token) => await sourceBroadcaster
                    .SetSourceMultiViewStreamAsync(cb!, token)
                    .ConfigureAwait(false),
                true,
                cancellationToken
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Attempts to get or create a source broadcaster internally, applying the provided <paramref name="setupBroadcaster"/> callback.
        /// </summary>
        private async Task<ISourceBroadcaster?> GetOrCreateSourceBroadcasterInternalAsync(
            SMStreamInfo smStreamInfo,
            IChannelBroadcaster? channelBroadcaster,
            Func<ISourceBroadcaster, IChannelBroadcaster?, CancellationToken, Task<long>> setupBroadcaster,
            bool reUseExisting,
            CancellationToken cancellationToken)
        {
            if (smStreamInfo == null)
            {
                return null;
            }
            await channelLockService.AcquireLockAsync(smStreamInfo.Id);
            try
            {
                if (reUseExisting && sourceBroadcasters.TryGetValue(smStreamInfo.Url, out ISourceBroadcaster? existingBroadcaster))
                {
                    if (existingBroadcaster.IsFailed)
                    {
                        await StopAndUnRegisterSourceBroadCasterAsync(smStreamInfo.Url).ConfigureAwait(false);
                    }
                    else
                    {
                        logger.LogInformation("Reusing source stream: {Id} {Name}", smStreamInfo.Id, smStreamInfo.Name);
                        return existingBroadcaster;
                    }
                }

                return await StartStreamAsync(smStreamInfo, channelBroadcaster, setupBroadcaster, cancellationToken);
            }
            finally
            {
                channelLockService.ReleaseLock(smStreamInfo.Id);
            }
        }

        private async Task<SourceBroadcaster?> StartStreamAsync(
            SMStreamInfo smStreamInfo,
            IChannelBroadcaster? channelBroadcaster,
            Func<ISourceBroadcaster, IChannelBroadcaster?, CancellationToken, Task<long>> setupBroadcaster,
            CancellationToken cancellationToken
            )
        {
            StreamConnectionMetricManager metrics = streamConnectionService.GetOrAdd(FileUtil.EncodeToMD5(smStreamInfo.Url), smStreamInfo.Url);

            SourceBroadcaster sourceBroadcaster = new(sourceBroadcasterLogger, metrics, settings, streamFactory, smStreamInfo, cancellationToken)
            {
                IsMultiView = channelBroadcaster != null
            };
            logger.LogInformation("Created new source stream for: {Id} {Name}", smStreamInfo.Id, smStreamInfo.Name);

            long connectionTime = await setupBroadcaster(sourceBroadcaster, channelBroadcaster, cancellationToken).ConfigureAwait(false);
            metrics.RecordConnectionAttempt();
            if (connectionTime == 0)
            {
                return null;
            }

            sourceBroadcaster.OnStreamBroadcasterStopped += OnSourceBroadcasterStoppedEvent;

            if (sourceBroadcasters.TryAdd(smStreamInfo.Url, sourceBroadcaster))
            {
                if (!smStreamInfo.Id.StartsWith(IntroPlayListBuilder.IntroIDPrefix, StringComparison.InvariantCulture))
                {
                    videoInfoService.SetSourceChannel(sourceBroadcaster, smStreamInfo.Id, smStreamInfo.Name);
                }
            }

            return sourceBroadcaster;
        }

        /// <inheritdoc />
        public async Task<bool> StopAndUnRegisterSourceBroadCasterAsync(string key)
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
                    streamConnectionService.Remove(key);
                    return true;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error stopping source broadcaster {Key}", key);
                }
            }

            return false;
        }

        /// <summary>
        /// Handles the event when a stream broadcaster stops, optionally retrying if conditions are met.
        /// </summary>
        private async Task OnSourceBroadcasterStoppedEvent(object? sender, SourceBroadcasterStopped e)
        {
            if (sender is not ISourceBroadcaster sourceBroadcaster)
            {
                return;
            }

            if (!sourceBroadcaster.IsFailed && !e.IsCancelled && !sourceBroadcaster.ChannelBroadcasters.IsEmpty && !sourceBroadcaster.IsMultiView)
            {
                int currentRetry = sourceBroadcaster.MetricManager.GetRetryCount();

                if (currentRetry >= settings.CurrentValue.StreamRetryLimit)
                {
                    logger.LogInformation("Stream {Name} retry limit ({currentRetry}) reached.", sourceBroadcaster.SMStreamInfo.Name, currentRetry);
                    sourceBroadcaster.MetricManager.RecordError();
                }
                else
                {
                    sourceBroadcaster.MetricManager.IncrementRetryCount();

                    logger.LogInformation("Retrying stream {Name} {RetryCount}/{RetryLimit}.",
                                      sourceBroadcaster.SMStreamInfo.Name, currentRetry, settings.CurrentValue.StreamRetryLimit);

                    List<KeyValuePair<string, IStreamDataToClients>> channelBroadcasters = [.. sourceBroadcaster.ChannelBroadcasters];
                    SMStreamInfo smStreamInfo = sourceBroadcaster.SMStreamInfo;

                    await StopAndUnRegisterSourceBroadCasterAsync(e.Id).ConfigureAwait(false);

                    ISourceBroadcaster? newBroadcaster = await GetOrCreateSourceBroadcasterInternalAsync(
                          smStreamInfo,
                          channelBroadcaster: null,
                          async (sourceBroadcaster, _, token) => await sourceBroadcaster
                              .SetSourceStreamAsync(smStreamInfo, token)
                              .ConfigureAwait(false),
                          true,
                          sourceBroadcaster.CancellationToken
                      ).ConfigureAwait(false);

                    if (newBroadcaster != null)
                    {
                        foreach (KeyValuePair<string, IStreamDataToClients> channelBroadcaster in channelBroadcasters)
                        {
                            newBroadcaster.AddChannelBroadcaster(channelBroadcaster.Key, channelBroadcaster.Value);
                        }
                    }
                    return;
                }
            }

            logger.LogInformation("Stream {Name} stopped.", sourceBroadcaster.SMStreamInfo.Name);

            await StopAndUnRegisterSourceBroadCasterAsync(e.Id).ConfigureAwait(false);

            if (OnStreamBroadcasterStopped != null)
            {
                await OnStreamBroadcasterStopped.Invoke(sender, e).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Checks all broadcasters; if any have zero active channels (excluding VideoInfo), schedule them for shutdown.
        /// </summary>
        private async Task CheckForEmptySourceBroadcastersAsync(CancellationToken cancellationToken = default)
        {
            foreach (ISourceBroadcaster sourceBroadcaster in sourceBroadcasters.Values)
            {
                int count = sourceBroadcaster.ChannelBroadcasters.Count(a => a.Key != "VideoInfo");
                if (count == 0)
                {
                    int delay = settings.CurrentValue.StreamShutDownDelayMs;
                    if (delay > 0)
                    {
                        logger.LogInformation("Delaying shutdown {StreamShutDownDelayMs}ms of source stream: {Id} {Name}", delay, sourceBroadcaster.SMStreamInfo.Id, sourceBroadcaster.SMStreamInfo.Name);
                        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                    }

                    count = sourceBroadcaster.ChannelBroadcasters.Count(a => a.Key != "VideoInfo");
                    if (count == 0)
                    {
                        await StopAndUnRegisterSourceBroadCasterAsync(sourceBroadcaster.Id).ConfigureAwait(false);
                    }
                }
            }
        }

        /// <inheritdoc />
        public async Task UnRegisterChannelBroadcasterAsync(int channelBroadcasterId)
        {
            ISourceBroadcaster? sourceBroadcaster = sourceBroadcasters.Values
                .FirstOrDefault(broadcaster => broadcaster
                    .ChannelBroadcasters
                    .ContainsKey(channelBroadcasterId.ToString()));

            if (sourceBroadcaster?.RemoveChannelBroadcaster(channelBroadcasterId) == true)
            {
                await CheckForEmptySourceBroadcastersAsync().ConfigureAwait(false);
            }
        }
    }
}