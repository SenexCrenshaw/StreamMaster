using StreamMaster.Application.Common.Models;

namespace StreamMaster.Application.Common.Interfaces
{
    public interface IStatisticsManager
    {
        List<Guid> GetAllClientIds();
        List<ClientStreamingStatistics> GetAllClientStatisticsByClientIds(ICollection<Guid> ClientIds);
        void AddBytesRead(Guid clientId, int count);
        List<ClientStreamingStatistics> GetAllClientStatistics();
        void IncrementBytesRead(Guid clientId);
        void RegisterClient(Guid clientId, string clientAgent, string clientIPAddress);
        void UnRegisterClient(Guid clientId);
    }
}