public interface IProxyFactory
{
    Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxy(IChannelStatus channelStatus, CancellationToken cancellationToken);
}
