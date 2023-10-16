using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces
{
    public interface IStatisticsManager
    {
        void AddBytesRead(Guid clientId, int count);
        List<ClientStreamingStatistics> GetAllClientStatistics();
        void IncrementBytesRead(Guid clientId);
        void RegisterClient(Guid clientId, string clientAgent, string clientIPAddress);
        void UnregisterClient(Guid clientId);
    }
}