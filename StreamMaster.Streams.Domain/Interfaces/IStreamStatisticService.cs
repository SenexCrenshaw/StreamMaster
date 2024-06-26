using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IStreamStatisticService
    {
        List<StreamStreamingStatistic> GetStreamStreamingStatistics();
        List<ChannelStreamingStatistics> GetChannelStreamingStatistics();
        Task<List<ClientStreamingStatistics>> GetClientStatistics(CancellationToken cancellationToken = default);
    }
}