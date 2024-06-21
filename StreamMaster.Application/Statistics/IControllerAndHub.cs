using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Statistics.Queries;

namespace StreamMaster.Application.Statistics
{
    public interface IStatisticsController
    {        
        Task<ActionResult<List<ClientStreamingStatistics>>> GetClientStreamingStatistics();
        Task<ActionResult<List<InputStreamingStatistics>>> GetInputStatistics();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IStatisticsHub
    {
        Task<List<ClientStreamingStatistics>> GetClientStreamingStatistics();
        Task<List<InputStreamingStatistics>> GetInputStatistics();
    }
}
