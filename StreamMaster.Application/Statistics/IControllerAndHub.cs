using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Statistics.Queries;

namespace StreamMaster.Application.Statistics
{
    public interface IStatisticsController
    {        
        Task<ActionResult<List<ChannelMetric>>> GetChannelMetrics();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IStatisticsHub
    {
        Task<List<ChannelMetric>> GetChannelMetrics();
    }
}
