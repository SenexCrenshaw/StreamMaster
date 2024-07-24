using StreamMaster.Domain.Models;

using System.Threading.Channels;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IStreamHandler
    {
        ChannelReader<byte[]> GetOutputChannelReader();
        //int ChannelCount { get; set; }
        //IEnumerable<IChannelStatus> GetChannelStatuses { get; }
        bool IsFailed { get; set; }
        SMStreamDto SMStream { get; }

        event EventHandler<StreamHandlerStopped> OnStreamingStoppedEvent;

        Task BuildVideoInfoAsync(byte[] videoMemory);
        void CancelStreamThread();
        void Dispose();
        double GetAverageLatency();
        long GetBytesRead();
        long GetBytesWritten();
        //IEnumerable<int> GetChannelStatusIds();

        int GetErrorCount();
        double GetKbps();
        DateTime GetStartTime();
        VideoInfo GetVideoInfo();
        //bool HasChannel(int smChannelId);
        bool IsHealthy();
        //void RegisterChannel(IChannelStatus channelStatus);
        void SetFailed();
        Task StartVideoStreamingAsync(Stream stream);
        void Stop(bool inputStreamError = false);
        //void UnRegisterAllChannels();
        //bool UnRegisterChannel(int smChannelId);
    }
}