using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Events;
using StreamMaster.Streams.Domain.Events;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Handlers
{
    public class VideoCombinerService(ILogger<VideoCombinerService> logger, IOptionsMonitor<Setting> _settings, IVideoCombiner videoCombiner, IServiceProvider serviceProvider)
        : IVideoCombinerService
    {
        public event AsyncEventHandler<VideoCombinerStopped>? OnVideoCombinerStoppedEvent;
        private readonly ConcurrentDictionary<string, IVideoCombiner> videoCombiners = new();
        private readonly SemaphoreSlim GetOrCreateVideoCombinerSlim = new(1, 1);

        public IDictionary<string, IStreamHandlerMetrics> GetMetrics()
        {
            Dictionary<string, IStreamHandlerMetrics> metrics = [];

            foreach (KeyValuePair<string, IVideoCombiner> kvp in videoCombiners)
            {
                IVideoCombiner channelDistributor = kvp.Value;
                metrics[kvp.Key] = channelDistributor.Metrics;
            }

            return metrics;
        }

        public async Task<IVideoCombiner?> GetOrCreateVideoCombinerAsync(IClientConfiguration config, IChannelService channelService, int SMChannelId1, int SMChannelId2, int SMChannelId3, int SMChannelId4, int streamGroupProfileId, CancellationToken cancellationToken)
        {
            await GetOrCreateVideoCombinerSlim.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                using IServiceScope scope = serviceProvider.CreateScope();
                IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

                SMChannel? smChannel1 = await repositoryWrapper.SMChannel.FirstOrDefaultAsync(a => a.Id == SMChannelId1);
                if (smChannel1 == null)
                {
                    logger.LogError("SMChannel1 {smChannel1} of the channels is not found", smChannel1);
                    return null;
                }
                SMChannel? smChannel2 = await repositoryWrapper.SMChannel.FirstOrDefaultAsync(a => a.Id == SMChannelId2);
                if (smChannel1 == null)
                {
                    logger.LogError("SMChannel2 {smChannel2} of the channels is not found", smChannel2);
                    return null;
                }
                SMChannel? smChannel3 = await repositoryWrapper.SMChannel.FirstOrDefaultAsync(a => a.Id == SMChannelId3);
                if (smChannel1 == null)
                {
                    logger.LogError("SMChannel3 {smChannel3} of the channels is not found", smChannel3);
                    return null;
                }
                SMChannel? smChannel4 = await repositoryWrapper.SMChannel.FirstOrDefaultAsync(a => a.Id == SMChannelId4);
                if (smChannel1 == null)
                {
                    logger.LogError("SMChannel4 {smChannel4} of the channels is not found", smChannel4);
                    return null;
                }

                IClientConfiguration config1 = config.DeepCopy();
                IClientConfiguration config2 = config.DeepCopy();
                IClientConfiguration config3 = config.DeepCopy();
                IClientConfiguration config4 = config.DeepCopy();

                config1.SetUniqueRequestId(config1.UniqueRequestId + "-1");
                config2.SetUniqueRequestId(config1.UniqueRequestId + "-2");
                config3.SetUniqueRequestId(config1.UniqueRequestId + "-3");
                config4.SetUniqueRequestId(config1.UniqueRequestId + "-4");


                IChannelBroadcaster? smChannel1Broadcaster = await channelService.GetOrCreateChannelBroadcasterAsync(config1, streamGroupProfileId);
                if (smChannel1Broadcaster == null)
                {
                    logger.LogError("SMChsmChannel1Broadcasterannel1 {smChannel1} getting stream failed", smChannel1);
                    return null;
                }
                IChannelBroadcaster? smChannel2Broadcaster = await channelService.GetOrCreateChannelBroadcasterAsync(config2, streamGroupProfileId);
                if (smChannel2Broadcaster == null)
                {
                    logger.LogError("SMChannel2 {smChannel2} getting stream failed", smChannel2);
                    return null;
                }
                IChannelBroadcaster? smChannel3Broadcaster = await channelService.GetOrCreateChannelBroadcasterAsync(config3, streamGroupProfileId);
                if (smChannel3Broadcaster == null)
                {
                    logger.LogError("SMChannel3 {smChannel3} getting stream failed", smChannel3);
                    return null;
                }
                IChannelBroadcaster? smChannel4Broadcaster = await channelService.GetOrCreateChannelBroadcasterAsync(config4, streamGroupProfileId);
                if (smChannel4Broadcaster == null)
                {
                    logger.LogError("SMChannel4 {smChannel4} getting stream failed", smChannel4);
                    return null;
                }

                //videoCombiner.SetSourceStream(stream, smStreamInfo.Name, cancellationToken);
                //videoCombiner.OnVideoCombinerStoppedEvent += OnStoppedEvent;

                //if (videoCombiners.TryAdd(smStreamInfo.Url, sourceBroadcaster))
                //{
                //    if (!smStreamInfo.Id.StartsWith(IntroPlayListBuilder.IntroIDPrefix, StringComparison.InvariantCulture))
                //    {
                //        _videoInfoService.SetSourceChannel(sourceBroadcaster, smStreamInfo.Url, smStreamInfo.Name);
                //    }
                //}

                //return sourceBroadcaster;
                //await videoCombiner.CombineVideosAsync(smChannel1Stream, smChannel2Stream, smChannel3Stream, smChannel4Stream, channelWriter, cancellationToken);
                return videoCombiner;
            }
            finally
            {
                GetOrCreateVideoCombinerSlim.Release();
            }
        }

        public IVideoCombiner? GetVideoCombiner(string? key)
        {
            return string.IsNullOrEmpty(key)
            ? null
               : !videoCombiners.TryGetValue(key, out IVideoCombiner? channelBroadcaster) ? null : channelBroadcaster;
        }

        public List<IVideoCombiner> GetVideoCombiners()
        {
            return [.. videoCombiners.Values];
        }

        public bool StopAndUnRegisterSourceBroadcaster(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

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
