using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Statistics.Queries;

namespace StreamMaster.Application.Statistics
{
    public interface IStatisticsController
    {        
        Task<ActionResult<List<ChannelMetric>>> GetChannelMetrics();
        Task<ActionResult<VideoInfo>> GetVideoInfo(GetVideoInfoRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IStatisticsHub
    {
        Task<List<ChannelMetric>> GetChannelMetrics();
        Task<VideoInfo> GetVideoInfo(GetVideoInfoRequest request);
    }
}
