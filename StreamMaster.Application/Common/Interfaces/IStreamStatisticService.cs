using StreamMaster.Application.Common.Models;

namespace StreamMaster.Application.Common.Interfaces
{
    public interface IStreamStatisticService
    {
        Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls(CancellationToken cancellationToken = default);
    }
}