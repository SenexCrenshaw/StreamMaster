namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IClientStatisticsManager
    {
        bool UnRegisterClient(Guid clientId);
        List<Guid> GetAllClientIds();
        //List<ClientStreamingStatistics> GetAllClientStatisticsByClientIds(ICollection<Guid> ClientIds);
        void AddBytesRead(Guid clientId, int bytesRead);
        List<ClientStreamingStatistics> GetAllClientStatistics();
        void IncrementBytesRead(Guid clientId);
        void RegisterClient(ClientStreamerConfiguration streamerConfiguration);
    }
}