using StreamMaster.Domain.Dto;

namespace StreamMaster.Domain.Services
{
    public interface IHLSManager : IDisposable
    {
        IHLSHandler? Get(string VideoStreamId);
        IHLSHandler GetOrAdd(VideoStreamDto videoStream);
        void Stop(string VideoStreamId);
    }
}