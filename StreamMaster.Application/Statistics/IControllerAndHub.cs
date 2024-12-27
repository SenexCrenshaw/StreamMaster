using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Statistics.Queries;

namespace StreamMaster.Application.Statistics
{
    public interface IStatisticsController
    {
        Task<ActionResult<List<ChannelMetric>>> GetChannelMetrics();
        Task<ActionResult<StreamConnectionMetricData>> GetStreamConnectionMetricData(GetStreamConnectionMetricDataRequest request);
        Task<ActionResult<List<StreamConnectionMetricData>>> GetStreamConnectionMetricDatas();
        Task<ActionResult<VideoInfo>> GetVideoInfo(GetVideoInfoRequest request);
        Task<ActionResult<List<VideoInfoDto>>> GetVideoInfos();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IStatisticsHub
    {
        Task<List<ChannelMetric>> GetChannelMetrics();
        Task<StreamConnectionMetricData> GetStreamConnectionMetricData(GetStreamConnectionMetricDataRequest request);
        Task<List<StreamConnectionMetricData>> GetStreamConnectionMetricDatas();
        Task<VideoInfo> GetVideoInfo(GetVideoInfoRequest request);
        Task<List<VideoInfoDto>> GetVideoInfos();
    }
}
