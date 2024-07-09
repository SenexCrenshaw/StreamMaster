using StreamMaster.Domain.Models;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IFFMPEGRunner
    {
        string FFMpegOptions { get; set; }
        int ProcessId { get; }

        event EventHandler<ProcessExitEventArgs> ProcessExited;

        Task<(Stream? stream, int processId, ProxyStreamError? error)> CreateFFMpegStream(string streamUrl, string streamName);
        Task<(Stream? stream, int processId, ProxyStreamError? error)> CreateFFMpegStreamByArgs(string args, string streamName);
        Task HLSStartStreamingInBackgroundAsync(SMChannel smChannel, string url, CancellationToken cancellationToken);
    }
}