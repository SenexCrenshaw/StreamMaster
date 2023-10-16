namespace StreamMasterApplication.Common.Interfaces
{
    public interface IProxyFactory
    {
        Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxy(string streamUrl);
    }
}