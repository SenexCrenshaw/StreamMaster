using System.Collections.Concurrent;

using StreamMaster.Domain.Events;
using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Domain.Metrics;

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
        IStreamFactory streamFactory)
        : ISourceBroadcasterService
    {
        private readonly ConcurrentDictionary<string, ISourceBroadcaster> sourceBroadcasters = new();
        private readonly SemaphoreSlim getOrCreateStreamDistributorSlim = new(1, 1);
        private readonly ConcurrentDictionary<string, StreamConnectionMetricManager> sourceStreamHandlerMetrics = new();

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

            await getOrCreateStreamDistributorSlim.WaitAsync(cancellationToken).ConfigureAwait(false);

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
                getOrCreateStreamDistributorSlim.Release();
            }
        }

        private async Task<SourceBroadcaster?> StartStreamAsync(
            SMStreamInfo smStreamInfo,
            IChannelBroadcaster? channelBroadcaster,
            Func<ISourceBroadcaster, IChannelBroadcaster?, CancellationToken, Task<long>> setupBroadcaster,
            CancellationToken cancellationToken
            )
        {
            SourceBroadcaster sourceBroadcaster = new(sourceBroadcasterLogger, settings, streamFactory, smStreamInfo)
            {
                IsMultiView = channelBroadcaster != null
            };
            logger.LogInformation("Created new source stream for: {Id} {Name}", smStreamInfo.Id, smStreamInfo.Name);

            StreamConnectionMetricManager metrics = sourceStreamHandlerMetrics.GetOrAdd(smStreamInfo.Url, _ => new StreamConnectionMetricManager(smStreamInfo.Id, smStreamInfo.Url));

            long connectionTime = await setupBroadcaster(sourceBroadcaster, channelBroadcaster, cancellationToken).ConfigureAwait(false);
            metrics.RecordConnectionAttempt(connectionTime: connectionTime);
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
                    sourceStreamHandlerMetrics.TryRemove(key, out _);
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

            StreamConnectionMetricManager metrics = sourceStreamHandlerMetrics[e.Id];
            int currentRetry = metrics.GetRetryCount();

            if (!e.IsCancelled && !sourceBroadcaster.IsMultiView && currentRetry < settings.CurrentValue.StreamRetryLimit)
            {
                metrics.IncrementRetryCount();

                logger.LogInformation("Retrying stream {Id} {RetryCount}/{RetryLimit}.",
                                  e.Id, currentRetry, settings.CurrentValue.StreamRetryLimit);

                List<KeyValuePair<string, IStreamDataToClients>> channelBroadcasters = [.. sourceBroadcaster.ChannelBroadcasters];
                SMStreamInfo smStreamInfo = sourceBroadcaster.SMStreamInfo;

                //await sourceBroadcaster.StopAsync().ConfigureAwait(false);
                //await Task.Delay(50);

                ISourceBroadcaster? newBroadcaster = await GetOrCreateSourceBroadcasterInternalAsync(
                    smStreamInfo,
                    channelBroadcaster: null,
                    async (newSourceBroadcaster, _, token) => await newSourceBroadcaster
                        .SetSourceStreamAsync(smStreamInfo, token)
                        .ConfigureAwait(false),
                    false,
                    CancellationToken.None
                ).ConfigureAwait(false);

                if (newBroadcaster != null)
                {
                    foreach (KeyValuePair<string, IStreamDataToClients> channelBroadcaster in channelBroadcasters)
                    {
                        newBroadcaster.AddChannelBroadcaster(channelBroadcaster.Key, channelBroadcaster.Value);
                    }
                }
            }
            else
            {
                if (currentRetry >= settings.CurrentValue.StreamRetryLimit)
                {
                    logger.LogInformation("Stream {Id} retry limit ({currentRetry}) reached.", e.Id, currentRetry);
                }
                else
                {
                    logger.LogInformation("Stream {Id} stopped.", e.Id);
                }

                await StopAndUnRegisterSourceBroadCasterAsync(e.Id).ConfigureAwait(false);

                if (OnStreamBroadcasterStopped != null)
                {
                    await OnStreamBroadcasterStopped.Invoke(sender, e).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Checks all broadcasters; if any have zero active channels (excluding VideoInfo), schedule them for shutdown.
        /// </summary>
        private async Task CheckForEmptyBroadcastersAsync(CancellationToken cancellationToken = default)
        {
            foreach (ISourceBroadcaster sourceBroadcaster in sourceBroadcasters.Values)
            {
                int count = sourceBroadcaster.ChannelBroadcasters.Count(a => a.Key != "VideoInfo");
                if (count == 0)
                {
                    int delay = settings.CurrentValue.StreamShutDownDelayMs;
                    if (delay > 0)
                    {
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

        public StreamConnectionMetricData? GetStreamConnectionMetricData(string key)
        {
            return sourceStreamHandlerMetrics.TryGetValue(key, out StreamConnectionMetricManager? metrics) ? metrics.MetricData : null;
        }

        public List<StreamConnectionMetricData> GetStreamConnectionMetrics()
        {
            return [.. sourceStreamHandlerMetrics.Values.Select(a => a.MetricData)];
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
                await CheckForEmptyBroadcastersAsync().ConfigureAwait(false);
            }
        }
    }
}
