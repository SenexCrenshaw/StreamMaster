

using System.Collections.Concurrent;
using System.Threading.Channels;

namespace StreamMaster.Streams.Plugins
{
    public class VideoInfoService : IVideoInfoService, IDisposable
    {
        private readonly ILogger<VideoInfoService> _logger;
        private readonly ILogger<VideoInfoPlugin> pluginLogger;
        private readonly IOptionsMonitor<Setting> _intSettings;

        public ConcurrentDictionary<string, VideoInfo> VideoInfos { get; } = new();
        public ConcurrentDictionary<string, VideoInfoPlugin> VideoInfoPlugins { get; } = new();

        public VideoInfoService(ILogger<VideoInfoService> logger, ILogger<VideoInfoPlugin> pluginLogger, IOptionsMonitor<Setting> intSettings)
        {
            _logger = logger;
            this.pluginLogger = pluginLogger;
            _intSettings = intSettings;
        }

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

        public void SetSourceChannel(IChannelBroadcaster sourceChannelBroadcaster, string Id)
        {
            Channel<byte[]> channelVideoInfo = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(200)
            {
                SingleReader = true,
                SingleWriter = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });

            //sourceChannelBroadcaster.AddClientChannel($"VideoInfo {sourceChannelBroadcaster.Id}", channelVideoInfo.Writer);
            sourceChannelBroadcaster.AddClientChannel("VideoInfo", channelVideoInfo.Writer);
            string key = sourceChannelBroadcaster.Name;

            if (!VideoInfoPlugins.TryGetValue(key, out VideoInfoPlugin? videoInfoPlugin))
            {
                _logger.LogInformation("Video info service started for {key}", key);
                videoInfoPlugin = new VideoInfoPlugin(pluginLogger, _intSettings, channelVideoInfo.Reader, key);
                videoInfoPlugin.VideoInfoUpdated += OnVideoInfoUpdated;
                VideoInfoPlugins.TryAdd(key, videoInfoPlugin);
            }
        }

        public bool RemoveSourceChannel(string key)
        {
            if (VideoInfoPlugins.TryRemove(key, out VideoInfoPlugin? videoInfoPlugin))
            {
                videoInfoPlugin.Stop();
                return true;
            }
            return false;
        }

        private void OnVideoInfoUpdated(object? sender, VideoInfoEventArgs e)
        {
            _logger.LogInformation("Video info got info for {key} {JsonOutput}", e.Key, e.VideoInfo.JsonOutput);
            VideoInfo updatedVideoInfo = e.VideoInfo;
            VideoInfos.AddOrUpdate(e.Key, updatedVideoInfo, (_, _) => updatedVideoInfo);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
