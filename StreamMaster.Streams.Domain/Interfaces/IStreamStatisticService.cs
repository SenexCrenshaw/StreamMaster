namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IStreamStatisticService
    {
        Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls(CancellationToken cancellationToken = default);
    }
}