using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces
{
    public interface IStreamStatisticService
    {
        Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls();
    }
}