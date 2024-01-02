namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IStatisticsManager
    {
        List<Guid> GetAllClientIds();
        List<ClientStreamingStatistics> GetAllClientStatisticsByClientIds(ICollection<Guid> ClientIds);
        void AddBytesRead(Guid clientId, int count);
        List<ClientStreamingStatistics> GetAllClientStatistics();
        void IncrementBytesRead(Guid clientId);
        void RegisterClient(IClientStreamerConfiguration streamerConfiguration);
        void UnRegisterClient(Guid clientId);
    }
}