using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IClientStatisticsManager
    {
        bool UnRegisterClient(string clientId);
        List<string> GetAllUniqueRequestIds();
        //List<ClientStreamingStatistics> GetAllClientStatisticsByClientIds(ICollection<Guid> ClientIds);
        void AddBytesRead(string clientId, int bytesRead);
        List<ClientStreamingStatistics> GetAllClientStatistics();
        void IncrementBytesRead(string clientId);
        void RegisterClient(ClientStreamerConfiguration streamerConfiguration);
    }
}