using StreamMaster.Domain.Models;

using System.Collections.Concurrent;
using System.Threading.Channels;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IVideoInfoService
    {
        ConcurrentDictionary<string, VideoInfo> VideoInfos { get; }
        bool HasVideoInfo(string key);
        void SetSourceChannel(ChannelReader<byte[]> channelReader, string key);
        bool RemoveSourceChannel(string key);
    }
}