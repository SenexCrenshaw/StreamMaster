namespace StreamMaster.Domain.Services
{
    public interface IHLSManager : IDisposable
    {
        IHLSHandler? Get(string VideoStreamId);
        Task<IHLSHandler> GetOrAdd(SMStream smStream);
        void Stop(string VideoStreamId);
    }
}