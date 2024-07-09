namespace StreamMaster.Domain.Services
{
    public interface IHLSManager : IDisposable
    {
        IHLSHandler? Get(int smChannelId);
        Task<IHLSHandler> GetOrAdd(SMChannelDto smChannel, string url);
        void Stop(int smChannelId);
    }
}