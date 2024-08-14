using System.Collections.Concurrent;
using System.Threading.Channels;

namespace StreamMaster.Streams.Plugins
{
    public class VideoInfoService(ILogger<VideoInfoService> logger, IDataRefreshService dataRefreshService, ILogger<VideoInfoPlugin> pluginLogger, IOptionsMonitor<Setting> intSettings) : IVideoInfoService, IDisposable
    {
        public ConcurrentDictionary<string, VideoInfo> VideoInfos { get; } = new();
        public ConcurrentDictionary<string, VideoInfoPlugin> VideoInfoPlugins { get; } = new();

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

        public VideoInfo? GetVideoInfos(string key)
        {
            return VideoInfos.TryGetValue(key, out VideoInfo? videoInfo) ? videoInfo : null;
        }

        public void SetSourceChannel(IChannelBroadcaster sourceChannelBroadcaster, string Id, string Name)
        {
            Channel<byte[]> channelVideoInfo = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(200)
            {
                SingleReader = true,
                SingleWriter = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });

            sourceChannelBroadcaster.AddClientChannel("VideoInfo", channelVideoInfo.Writer);

            if (!VideoInfoPlugins.TryGetValue(Id, out VideoInfoPlugin? videoInfoPlugin))
            {
                logger.LogInformation("Video info service started for {Id}", Id);
                videoInfoPlugin = new VideoInfoPlugin(pluginLogger, intSettings, channelVideoInfo.Reader, Id, Name);
                videoInfoPlugin.VideoInfoUpdated += OnVideoInfoUpdated;
                VideoInfoPlugins.TryAdd(Id, videoInfoPlugin);
            }
        }

        public bool RemoveSourceChannel(string Id)
        {
            if (VideoInfoPlugins.TryRemove(Id, out VideoInfoPlugin? videoInfoPlugin))
            {
                videoInfoPlugin.Stop();
                return true;
            }
            return false;
        }

        private void OnVideoInfoUpdated(object? sender, VideoInfoEventArgs e)
        {
            logger.LogInformation("Video info got info for {key} {JsonOutput}", e.Id, e.VideoInfo.JsonOutput);
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
