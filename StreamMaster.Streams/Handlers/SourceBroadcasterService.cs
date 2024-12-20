using System.Collections.Concurrent;

using StreamMaster.Domain.Events;
using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Handlers
{
    public class SourceBroadcasterService(ILogger<SourceBroadcasterService> logger, IVideoInfoService _videoInfoService, ILogger<ISourceBroadcaster> sourceBroadcasterLogger, IOptionsMonitor<Setting> _settings, IStreamFactory streamFactory)
        : ISourceBroadcasterService
    {
        public event AsyncEventHandler<StreamBroadcasterStopped>? OnStreamBroadcasterStoppedEvent;

        private readonly ConcurrentDictionary<string, ISourceBroadcaster> sourceBroadcasters = new();
        private readonly SemaphoreSlim GetOrCreateStreamDistributorSlim = new(1, 1);

        public ISourceBroadcaster? GetStreamBroadcaster(string? key)
        {
            return string.IsNullOrEmpty(key)
                ? null
                : !sourceBroadcasters.TryGetValue(key, out ISourceBroadcaster? channelBroadcaster) ? null : channelBroadcaster;
        }

        public List<ISourceBroadcaster> GetStreamBroadcasters()
        {
            return [.. sourceBroadcasters.Values];
        }

        public async Task<ISourceBroadcaster?> GetOrCreateStreamBroadcasterAsync(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken)
        {
            if (channelBroadcaster.SMStreamInfo == null)
            {
                return null;
            }

            SMStreamInfo smStreamInfo = channelBroadcaster.SMStreamInfo;

            await GetOrCreateStreamDistributorSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (sourceBroadcasters.TryGetValue(smStreamInfo.Url, out ISourceBroadcaster? sourceBroadcaster))
                {
                    if (sourceBroadcaster.IsFailed)
                    {
                        _ = await StopAndUnRegisterSourceBroadcasterAsync(smStreamInfo.Url);
                        _ = sourceBroadcasters.TryGetValue(smStreamInfo.Url, out sourceBroadcaster);
                    }

                    logger.LogInformation("Reusing source stream: {Id} {name}", smStreamInfo.Id, smStreamInfo.Name);
                    return sourceBroadcaster;
                }

                sourceBroadcaster = new SourceBroadcaster(sourceBroadcasterLogger, streamFactory, smStreamInfo);

                logger.LogInformation("Created new source stream for: {Id} {name}", smStreamInfo.Id, smStreamInfo.Name);

                await sourceBroadcaster.SetSourceStreamAsync(channelBroadcaster, cancellationToken);

                sourceBroadcaster.OnStreamBroadcasterStoppedEvent += OnStoppedEvent;

                if (sourceBroadcasters.TryAdd(smStreamInfo.Url, sourceBroadcaster))
                {
                    if (!smStreamInfo.Id.StartsWith(IntroPlayListBuilder.IntroIDPrefix, StringComparison.InvariantCulture))
                    {
                        _videoInfoService.SetSourceChannel(sourceBroadcaster, smStreamInfo.Id, smStreamInfo.Name);
                    }
                }

                return sourceBroadcaster;
            }
            finally
            {
                GetOrCreateStreamDistributorSlim.Release();
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
                ICollection<System.IO.Pipelines.PipeWriter> a = sourceBroadcaster.ChannelBroadcasters.Values;
                await sourceBroadcaster.StopAsync();
                _videoInfoService.StopVideoPlugin(sourceBroadcaster.SMStreamInfo.Id);
                return true;
            }

            return false;
        }

        private void OnStoppedEvent(object? sender, StreamBroadcasterStopped e)
        {
            if (sender is ISourceBroadcaster channelBroadcaster)
            {
                StopAndUnRegisterSourceBroadcasterAsync(e.Id).Wait();
                OnStreamBroadcasterStoppedEvent?.Invoke(sender!, e);
            }
        }

        //
        private async Task CheckForEmptyBroadcastersAsync(CancellationToken cancellationToken = default)
        {
            foreach (ISourceBroadcaster? sourceBroadcaster in sourceBroadcasters.Values)
            {
                int count = sourceBroadcaster.ChannelBroadcasters.Count(a => a.Key != "VideoInfo");
                if (count == 0)
                {
                    int delay = _settings.CurrentValue.ShutDownDelay;
                    if (delay > 0)
                    {
                        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                    }
                    count = sourceBroadcaster.ChannelBroadcasters.Count(a => a.Key != "VideoInfo");
                    if (count != 0)
                    {
                        return;
                    }

                    //await sourceBroadcaster.StopAsync();
                    await StopAndUnRegisterSourceBroadcasterAsync(sourceBroadcaster.Id);
                }
            }
        }

        public async Task UnRegisterChannelBroadcasterAsync(int channelBroadcasterId)
        {
            ISourceBroadcaster? sourceBroadcaster = sourceBroadcasters.Values.FirstOrDefault(broadcaster => broadcaster.ChannelBroadcasters.ContainsKey(channelBroadcasterId.ToString()));
            if (sourceBroadcaster == null)
            {
                return;
            }

            if (sourceBroadcaster.RemoveChannelBroadcaster(channelBroadcasterId))
            {
                await CheckForEmptyBroadcastersAsync();
            }
        }
    }
}