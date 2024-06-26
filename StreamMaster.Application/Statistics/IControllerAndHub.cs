using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Statistics.Queries;

namespace StreamMaster.Application.Statistics
{
    public interface IStatisticsController
    {        
        Task<ActionResult<List<ChannelStreamingStatistics>>> GetChannelStreamingStatistics();
        Task<ActionResult<List<ClientStreamingStatistics>>> GetClientStreamingStatistics();
        Task<ActionResult<List<StreamStreamingStatistic>>> GetStreamingStatisticsForChannel(GetStreamingStatisticsForChannelRequest request);
        Task<ActionResult<List<StreamStreamingStatistic>>> GetStreamStreamingStatistics();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IStatisticsHub
    {
        Task<List<ChannelStreamingStatistics>> GetChannelStreamingStatistics();
        Task<List<ClientStreamingStatistics>> GetClientStreamingStatistics();
        Task<List<StreamStreamingStatistic>> GetStreamingStatisticsForChannel(GetStreamingStatisticsForChannelRequest request);
        Task<List<StreamStreamingStatistic>> GetStreamStreamingStatistics();
    }
}
