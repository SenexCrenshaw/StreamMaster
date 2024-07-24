namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface ISMStream
    {
        Task<(Stream? stream, int processId, ProxyStreamError? error)> HandleStream(IChannelStatus channelStatus, string clientUserAgent, CancellationToken cancellationToken);
    }
}