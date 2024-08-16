using StreamMaster.Domain.Events;
using StreamMaster.Streams.Domain.Events;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Services
{
    /// <summary>
    /// Service for managing the status of channels, including creation, monitoring, and disposal.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ChannelStatusService"/> class.
    /// </remarks>
    /// <param name="logger">Logger for this service.</param>
    /// <param name="dubcer">Service responsible for handling dubc operations.</param>
    /// <param name="channelStatusLogger">Logger for channel status.</param>
    /// <param name="videoInfoService">Service for managing video information.</param>
    public class ChannelStatusService(
        ILogger<ChannelStatusService> logger,
        IDubcer dubcer,
        ILogger<IChannelBroadcaster> channelStatusLogger,
        IVideoInfoService videoInfoService) : IChannelStatusService
    {
        /// <inheritdoc/>
        public event AsyncEventHandler<ChannelStatusStopped>? OnChannelStatusStoppedEvent;

        private readonly ConcurrentDictionary<int, IChannelStatus> _sourceChannelDistributors = new();
        private readonly SemaphoreSlim _getOrCreateSourceChannelDistributorSlim = new(1, 1);

        /// <inheritdoc/>
        public IDictionary<int, IStreamHandlerMetrics> GetMetrics()
        {
            Dictionary<int, IStreamHandlerMetrics> metrics = [];

            foreach (KeyValuePair<int, IChannelStatus> kvp in _sourceChannelDistributors)
            {
                IChannelStatus channelDistributor = kvp.Value;
                metrics[kvp.Key] = channelDistributor.Metrics;
            }

            return metrics;
        }

        /// <inheritdoc/>
        public async Task<IChannelStatus> GetOrCreateChannelStatusAsync(IClientConfiguration config, int streamGroupProfileId, CancellationToken cancellationToken)
        {
            await _getOrCreateSourceChannelDistributorSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_sourceChannelDistributors.TryGetValue(config.SMChannel.Id, out IChannelStatus? channelStatus))
                {
                    if (channelStatus.IsFailed)
                    {
                        StopAndUnRegisterChannelStatus(config.SMChannel.Id);
                    }
                    else
                    {
                        logger.LogInformation("Reusing channel distributor: {Id} {Name}", config.SMChannel.Id, config.SMChannel.Name);
                        return channelStatus;
                    }
                }

                channelStatus = new ChannelStatus(channelStatusLogger, dubcer, config.SMChannel)
                {
                    SMChannel = config.SMChannel,
                    StreamGroupProfileId = streamGroupProfileId
                };

                logger.LogInformation("Created new channel for: {Id} {Name}", config.SMChannel.Id, config.SMChannel.Name);

                channelStatus.OnChannelStatusStoppedEvent += OnChannelStatusStopped;
                _sourceChannelDistributors.TryAdd(config.SMChannel.Id, channelStatus);

                return channelStatus;
            }
            finally
            {
                _getOrCreateSourceChannelDistributorSlim.Release();
            }
        }

        /// <inheritdoc/>
        public bool StopAndUnRegisterChannelStatus(int key)
        {
            if (_sourceChannelDistributors.TryRemove(key, out IChannelStatus? channelStatus))
            {
                channelStatus.Stop();
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public List<IChannelStatus> GetChannelStatuses()
        {
            return [.. _sourceChannelDistributors.Values];
        }

        /// <inheritdoc/>
        private void OnChannelStatusStopped(object? sender, ChannelStatusStopped e)
        {
            if (sender is IChannelStatus channelStatus)
            {
                channelStatus.Shutdown = true;
                OnChannelStatusStoppedEvent?.Invoke(sender, e);
                StopAndUnRegisterChannelStatus(e.Id);
                videoInfoService.RemoveSourceChannel(e.Name);
            }
        }
    }
}
