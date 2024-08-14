using StreamMaster.Domain.Models;


using System.Collections.Concurrent;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IVideoInfoService
    {
        ConcurrentDictionary<string, VideoInfo> VideoInfos { get; }
        bool HasVideoInfo(string key);
        void SetSourceChannel(IChannelBroadcaster sourceChannelBroadcaster, string Id);
        bool RemoveSourceChannel(string key);
    }
}