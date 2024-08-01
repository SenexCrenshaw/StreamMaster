using System.Collections.Concurrent;
using System.Threading.Channels;

namespace StreamMaster.Streams.Plugins
{
    public class VideoInfoService(ILogger<VideoInfoPlugin> infoLogger, IOptionsMonitor<Setting> intSettings)
        : IVideoInfoService, IDisposable
    {
        public ConcurrentDictionary<string, VideoInfo> VideoInfos { get; } = new();
        public ConcurrentDictionary<string, VideoInfoPlugin> VideoInfoPlugins { get; } = new();

        public void Stop()
        {
            foreach (KeyValuePair<string, VideoInfoPlugin> videoInfoPlugin in VideoInfoPlugins)
            {
                videoInfoPlugin.Value.Stop();
            }
        }
        public bool HasVideoInfo(string key)
        {
            return VideoInfos.ContainsKey(key);
        }
        public void SetSourceChannel(ChannelReader<byte[]> channelReader, string key)
        {
            if (!VideoInfoPlugins.TryGetValue(key, out VideoInfoPlugin _videoInfoPlugin))
            {
                _videoInfoPlugin = new VideoInfoPlugin(infoLogger, intSettings, channelReader, key);
                _videoInfoPlugin.VideoInfoUpdated += OnVideoInfoUpdated;
                VideoInfoPlugins.TryAdd(key, _videoInfoPlugin);
            }
        }
        public bool RemoveSourceChannel(string key)
        {
            if (VideoInfoPlugins.TryRemove(key, out VideoInfoPlugin videoInfoPlugin))
            {
                videoInfoPlugin.Stop();
                return true;
            }
            return false;
        }

        private void OnVideoInfoUpdated(object? sender, VideoInfoEventArgs e)
        {
            VideoInfo updatedVideoInfo = e.VideoInfo;
            VideoInfos.AddOrUpdate(e.Key, updatedVideoInfo, (_, _) => updatedVideoInfo);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}