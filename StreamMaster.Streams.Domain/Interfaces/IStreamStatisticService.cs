namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IStreamStatisticService
    {
        Task<List<InputStreamingStatistics>> GetInputStatistics(CancellationToken cancellationToken = default);
        Task<List<ClientStreamingStatistics>> GetClientStatistics(CancellationToken cancellationToken = default);
    }
}