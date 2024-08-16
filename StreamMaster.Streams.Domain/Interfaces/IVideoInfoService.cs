using StreamMaster.Domain.Models;


using System.Collections.Concurrent;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IVideoInfoService
    {
        VideoInfo? GetVideoInfo(string smStreamId);
        ConcurrentDictionary<string, VideoInfo> VideoInfos { get; }
        bool HasVideoInfo(string key);
        void SetSourceChannel(IStreamBroadcaster sourceChannelBroadcaster, string Id, string Name);
        bool RemoveSourceChannel(string key);
    }
}