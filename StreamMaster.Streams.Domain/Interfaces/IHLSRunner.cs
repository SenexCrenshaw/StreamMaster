using StreamMaster.Streams.Domain.Args;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IHLSRunner
    {
        public IM3U8ChannelStatus ChannelStatus { get; }

        int ProcessId { get; }

        event EventHandler<ProcessExitEventArgs> ProcessExited;

        //Task<(Stream? stream, int processId, ProxyStreamError? error)> CreateFFMpegStream(string streamUrl, string streamName);
        //Task<(Stream? stream, int processId, ProxyStreamError? error)> CreateFFMpegStreamByArgs(string args, string streamName);
        Task HLSStartStreamingInBackgroundAsync(CancellationToken cancellationToken);
    }
}