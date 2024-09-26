using StreamMaster.Domain.Models;

public interface IStreamFactory
{
    //Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxy(IChannelBroadcaster channelStatus, CancellationToken cancellationToken);
    Task<(Stream? stream, int processId, ProxyStreamError? error)> GetStream(SMStreamInfo smStreamInfo, CancellationToken cancellationToken);
}
