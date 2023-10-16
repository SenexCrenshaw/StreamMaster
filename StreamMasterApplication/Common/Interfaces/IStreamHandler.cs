using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces;

public interface IStreamHandler : IDisposable
{
    Task StartVideoStreamingAsync(Stream stream, ICircularRingBuffer circularRingbuffer);

    bool FailoverInProgress { get; set; }
    int M3UFileId { get; set; }
    bool M3UStream { get; set; }
    int MaxStreams { get; set; }
    int ProcessId { get; set; }
    ICircularRingBuffer RingBuffer { get; }
    string StreamUrl { get; set; }
    CancellationTokenSource VideoStreamingCancellationToken { get; set; }
    int ClientCount { get; }
    void Dispose();
    void RegisterClientStreamer(ClientStreamerConfiguration streamerConfiguration);
    void Stop();
    bool UnRegisterClientStreamer(ClientStreamerConfiguration streamerConfiguration);
    ClientStreamerConfiguration? GetClientStreamerConfiguration(Guid client);
    ICollection<ClientStreamerConfiguration>? GetClientStreamerConfigurations();
}
