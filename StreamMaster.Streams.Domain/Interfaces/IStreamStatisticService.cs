namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IStreamStatisticService
    {
        List<InputStreamingStatistics> GetInputStatistics();
        Task<List<ClientStreamingStatistics>> GetClientStatistics(CancellationToken cancellationToken = default);
    }
}