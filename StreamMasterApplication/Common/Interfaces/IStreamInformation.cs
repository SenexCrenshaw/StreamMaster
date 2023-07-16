using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces
{
    public interface IStreamInformation
    {
        int ClientCount { get; }
        bool FailoverInProgress { get; set; }
        int M3UFileId { get; set; }
        bool M3UStream { get; set; }
        int MaxStreams { get; set; }
        int ProcessId { get; set; }
        ICircularRingBuffer RingBuffer { get; }
        Task StreamingTask { get; set; }        
        string StreamUrl { get; set; }
        CancellationTokenSource VideoStreamingCancellationToken { get; set; }

        void Dispose();

        ClientStreamerConfiguration? GetStreamConfiguration(Guid ClientId);

        List<ClientStreamerConfiguration> GetStreamConfigurations();

        void RegisterStreamConfiguration(ClientStreamerConfiguration streamerConfiguration);

        void Stop();

        bool UnRegisterStreamConfiguration(ClientStreamerConfiguration streamerConfiguration);
    }
}
