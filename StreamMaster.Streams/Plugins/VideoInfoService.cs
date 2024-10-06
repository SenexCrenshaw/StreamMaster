using StreamMaster.Streams.Domain.Helpers;

using System.Collections.Concurrent;
using System.Threading.Channels;

namespace StreamMaster.Streams.Plugins
{
    public class VideoInfoService(ILogger<VideoInfoService> logger, IDataRefreshService dataRefreshService, ILogger<VideoInfoPlugin> pluginLogger, IOptionsMonitor<Setting> intSettings)
        : IVideoInfoService
    {
        public ConcurrentDictionary<string, VideoInfo> VideoInfos { get; } = new();
        private ConcurrentDictionary<string, VideoInfoPlugin> VideoInfoPlugins { get; } = new();

        public void Stop()
        {
            foreach (VideoInfoPlugin videoInfoPlugin in VideoInfoPlugins.Values)
            {
                videoInfoPlugin.Stop();
            }
        }

        public bool HasVideoInfo(string key)
        {
            return VideoInfos.ContainsKey(key);
        }

        public VideoInfo? GetVideoInfo(string key)
        {
            return VideoInfos.TryGetValue(key, out VideoInfo? videoInfo) ? videoInfo : null;
        }

        public IEnumerable<VideoInfo> GetVideoInfos()
        {
            return VideoInfos.Values;
        }

        public void SetSourceChannel(ISourceBroadcaster sourceBroadcaster, string Id, string Name)
        {
            //Channel<byte[]> channelVideoInfo = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(200)
            //{
            //    SingleReader = true,
            //    SingleWriter = true,
            //    FullMode = BoundedChannelFullMode.DropOldest
            //});

            Domain.TrackedChannel channel = ChannelHelper.GetChannel(200, BoundedChannelFullMode.DropOldest);

            sourceBroadcaster.AddChannelStreamer("VideoInfo", channel);

            if (!VideoInfoPlugins.TryGetValue(Id, out VideoInfoPlugin? videoInfoPlugin))
            {
                logger.LogInformation("Video info service started for {Name}", Name);
                videoInfoPlugin = new VideoInfoPlugin(pluginLogger, intSettings, channel, Id, Name);
                videoInfoPlugin.VideoInfoUpdated += OnVideoInfoUpdated;
                VideoInfoPlugins.TryAdd(Id, videoInfoPlugin);
            }
        }

        public bool StopVideoPlugin(string Id)
        {
            if (VideoInfoPlugins.TryRemove(Id, out VideoInfoPlugin? videoInfoPlugin))
            {
                VideoInfos.TryRemove(Id, out _);
                videoInfoPlugin.Stop();
                return true;
            }
            return false;
        }

        private void OnVideoInfoUpdated(object? sender, VideoInfoEventArgs e)
        {
            logger.LogDebug("Video info got info for {key} {JsonOutput}", e.Id, e.VideoInfo.JsonOutput);
            VideoInfo updatedVideoInfo = e.VideoInfo;
            VideoInfos.AddOrUpdate(e.Id, updatedVideoInfo, (_, _) => updatedVideoInfo);
            dataRefreshService.RefreshVideoInfos().Wait();
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}
