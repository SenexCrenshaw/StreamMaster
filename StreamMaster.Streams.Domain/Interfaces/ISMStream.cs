using StreamMaster.Domain.Models;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface ISMStream
    {
        Task<(Stream? stream, int processId, ProxyStreamError? error)> HandleStream(SMStreamInfo smStreamInfo, string clientUserAgent, CancellationToken cancellationToken);
    }
}