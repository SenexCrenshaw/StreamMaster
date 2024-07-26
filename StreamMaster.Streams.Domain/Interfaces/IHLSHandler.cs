using StreamMaster.Streams.Domain.Args;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface IHLSHandler : IDisposable
{
    event EventHandler<ProcessExitEventArgs> ProcessExited;
    IM3U8ChannelStatus ChannelStatus { get; }
    void Stop();
}