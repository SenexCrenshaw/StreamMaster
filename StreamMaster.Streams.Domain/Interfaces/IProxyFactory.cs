using StreamMaster.Domain.Models;

public interface IProxyFactory
{
    //Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxy(IChannelBroadcaster channelStatus, CancellationToken cancellationToken);
    Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxy(SMStreamInfo smStreamInfo, CancellationToken cancellationToken);
}
