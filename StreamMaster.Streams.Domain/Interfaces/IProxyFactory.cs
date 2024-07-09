public interface IProxyFactory
{
    Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxy(SMStreamDto smStream, string clientUserAgent, CancellationToken cancellationToken);
}
