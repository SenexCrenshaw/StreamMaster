using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Statistics.Queries;

namespace StreamMaster.Application.Statistics
{
    public interface IStatisticsController
    {
        Task<ActionResult<List<ChannelMetric>>> GetChannelMetrics();
        Task<ActionResult<StreamConnectionMetric>> GetStreamConnectionMetric(GetStreamConnectionMetricRequest request);
        Task<ActionResult<List<StreamConnectionMetric>>> GetStreamConnectionMetrics();
        Task<ActionResult<VideoInfo>> GetVideoInfo(GetVideoInfoRequest request);
        Task<ActionResult<List<VideoInfoDto>>> GetVideoInfos();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IStatisticsHub
    {
        Task<List<ChannelMetric>> GetChannelMetrics();
        Task<StreamConnectionMetric> GetStreamConnectionMetric(GetStreamConnectionMetricRequest request);
        Task<List<StreamConnectionMetric>> GetStreamConnectionMetrics();
        Task<VideoInfo> GetVideoInfo(GetVideoInfoRequest request);
        Task<List<VideoInfoDto>> GetVideoInfos();
    }
}
