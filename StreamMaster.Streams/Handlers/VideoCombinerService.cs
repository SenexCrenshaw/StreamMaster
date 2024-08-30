using AutoMapper;

using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Events;
using StreamMaster.Streams.Domain.Events;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Handlers
{
    public class VideoCombinerService(ILogger<IBroadcasterBase> logger, IClientConfigurationService clientConfigurationService, IOptionsMonitor<Setting> _settings, IServiceProvider serviceProvider)
        : IVideoCombinerService
    {
        public event AsyncEventHandler<VideoCombinerStopped>? OnVideoCombinerStoppedEvent;
        private readonly ConcurrentDictionary<int, IVideoCombiner> videoCombiners = new();
        private readonly SemaphoreSlim GetOrCreateVideoCombinerSlim = new(1, 1);

        public IDictionary<string, IStreamHandlerMetrics> GetMetrics()
        {
            Dictionary<string, IStreamHandlerMetrics> metrics = [];

            foreach (KeyValuePair<int, IVideoCombiner> kvp in videoCombiners)
            {
                IVideoCombiner channelDistributor = kvp.Value;
                metrics[kvp.Key.ToString()] = channelDistributor.Metrics;
            }

            return metrics;
        }

        public async Task<IVideoCombiner?> GetOrCreateVideoCombinerAsync(IClientConfiguration config, IMapper mapper, IChannelService channelService, int SMChannelId1, int SMChannelId2, int SMChannelId3, int SMChannelId4, int streamGroupProfileId, CancellationToken cancellationToken)
        {
            if (videoCombiners.TryGetValue(config.SMChannel.Id, out IVideoCombiner? videoCombiner))
            {
                return videoCombiner;
            }

            await GetOrCreateVideoCombinerSlim.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                using IServiceScope scope = serviceProvider.CreateScope();
                IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

                IChannelBroadcaster? smChannelBroadcaster1 = await GetBroadcaster(repositoryWrapper, channelService, SMChannelId1, streamGroupProfileId, config, cancellationToken);
                if (smChannelBroadcaster1 == null)
                {
                    return null;
                }

                IChannelBroadcaster? smChannelBroadcaster2 = await GetBroadcaster(repositoryWrapper, channelService, SMChannelId2, streamGroupProfileId, config, cancellationToken);
                if (smChannelBroadcaster2 == null)
                {
                    return null;
                }

                IChannelBroadcaster? smChannelBroadcaster3 = await GetBroadcaster(repositoryWrapper, channelService, SMChannelId3, streamGroupProfileId, config, cancellationToken);
                if (smChannelBroadcaster3 == null)
                {
                    return null;
                }

                IChannelBroadcaster? smChannelBroadcaster4 = await GetBroadcaster(repositoryWrapper, channelService, SMChannelId4, streamGroupProfileId, config, cancellationToken);
                if (smChannelBroadcaster4 == null)
                {
                    return null;
                }

                videoCombiner = new VideoCombiner(logger, config.SMChannel.Id, config.SMChannel.Name, _settings);

                await videoCombiner.CombineVideosAsync(smChannelBroadcaster1, smChannelBroadcaster2, smChannelBroadcaster3, smChannelBroadcaster4, cancellationToken);

                videoCombiners.TryAdd(config.SMChannel.Id, videoCombiner);

                return videoCombiner;
            }
            finally
            {
                GetOrCreateVideoCombinerSlim.Release();
            }
        }

        public async Task<IChannelBroadcaster?> GetBroadcaster(IRepositoryWrapper repositoryWrapper, IChannelService channelService, int smChannelId, int streamGroupProfileId, IClientConfiguration config, CancellationToken cancellationToken)
        {
            SMChannel? smChannel = await repositoryWrapper.SMChannel.FirstOrDefaultAsync(a => a.Id == smChannelId, cancellationToken: cancellationToken);
            if (smChannel == null)
            {
                logger.LogError("GetBroadcaster {smChannelId} not found", smChannelId);
                return null;
            }
            IClientConfiguration newConfig = clientConfigurationService.Copy(config);

            IChannelBroadcaster? smChannel1Broadcaster = await channelService.GetOrCreateChannelBroadcasterAsync(newConfig, streamGroupProfileId);
            if (smChannel1Broadcaster == null)
            {
                logger.LogError("GetBroadcaster {smChannelId} getting stream failed", smChannelId);
                return null;
            }
            return smChannel1Broadcaster;
        }

        public IVideoCombiner? GetVideoCombiner(int key)
        {
            return videoCombiners.TryGetValue(key, out IVideoCombiner? channelBroadcaster) ? null : channelBroadcaster;
        }

        public List<IVideoCombiner> GetVideoCombiners()
        {
            return [.. videoCombiners.Values];
        }

        public bool StopAndUnRegisterSourceBroadcaster(int key)
        {
            if (videoCombiners.TryRemove(key, out IVideoCombiner? videoCombiner))
            {
                videoCombiner.Stop();
                return true;
            }

            return false;
        }

        private async Task CheckForEmptyVideoCombinerAsync(CancellationToken cancellationToken = default)
        {
            foreach (IVideoCombiner? videoCombiner in videoCombiners.Values)
            {
                int count = videoCombiner.ClientChannelWriters.Count(a => a.Key != "VideoInfo");
                if (count == 0)
                {
                    int delay = _settings.CurrentValue.ShutDownDelay;
                    if (delay > 0)
                    {
                        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                    }
                    count = videoCombiner.ClientChannelWriters.Count(a => a.Key != "VideoInfo");
                    if (count != 0)
                    {
                        return;
                    }

                    videoCombiner.Stop();
                    //StopAndUnRegisterSourceBroadcaster(videoCombiner.Id);

                }
            }
        }

        public async Task UnRegisterChannelBroadcasterAsync(int channelBroadcasterId)
        {
            IVideoCombiner? videoCombiner = videoCombiners.Values.FirstOrDefault(broadcaster => broadcaster.ClientChannelWriters.ContainsKey(channelBroadcasterId.ToString()));
            if (videoCombiner == null)
            {
                return;
            }

            if (videoCombiner.ClientChannelWriters.TryRemove(channelBroadcasterId.ToString(), out _))
            {
                await CheckForEmptyVideoCombinerAsync();
            }
        }
    }
}
