using StreamMaster.Streams.Domain.Statistics;

using System.Collections.Concurrent;
using System.Threading.Channels;

namespace StreamMaster.Streams.Domain.Interfaces
{

    public interface IChannelDistributor : IStreamStats
    {
        SMStreamInfo SMStreamInfo { get; }
        string SourceName { get; }
        event EventHandler<ChannelDirectorStopped> OnStoppedEvent;
        long GetChannelItemCount { get; }
        bool IsChannelEmpty();
        ConcurrentDictionary<string, ChannelWriter<byte[]>> ClientChannels { get; }
        void AddClientChannel(string key, ChannelWriter<byte[]> Channel);
        void AddClientChannel(int key, ChannelWriter<byte[]> Channel);
        bool RemoveClientChannel(string key);
        bool RemoveClientChannel(int key);

        ConcurrentDictionary<string, Stream> ClientStreams { get; }
        void AddClientStream(string key, Stream stream);
        void AddClientStream(int key, Stream stream);
        bool RemoveClientStream(string key);
        bool RemoveClientStream(int key);

        void SetSourceChannel(ChannelReader<byte[]> sourceChannelReader, string Name, CancellationToken cancellationToken);
        void SetSourceStream(Stream sourceStream, string Name, CancellationToken cancellationToken);
        void Stop();
        StreamHandlerMetrics GetMetrics { get; }

        public bool IsFailed { get; }
    }
}