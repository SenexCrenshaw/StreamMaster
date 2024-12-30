using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Statistics.Queries;

namespace StreamMaster.Application.Statistics
{
    public interface IStatisticsController
    {
        Task<ActionResult<List<ChannelMetric>>> GetChannelMetrics();
        Task<ActionResult<ImageDownloadServiceStatus>> GetDownloadServiceStatus();
        Task<ActionResult<bool>> GetIsSystemReady();
        Task<ActionResult<StreamConnectionMetricData>> GetStreamConnectionMetricData(GetStreamConnectionMetricDataRequest request);
        Task<ActionResult<List<StreamConnectionMetricData>>> GetStreamConnectionMetricDatas();
        Task<ActionResult<SDSystemStatus>> GetSystemStatus();
        Task<ActionResult<bool>> GetTaskIsRunning();
        Task<ActionResult<VideoInfo>> GetVideoInfo(GetVideoInfoRequest request);
        Task<ActionResult<List<VideoInfoDto>>> GetVideoInfos();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IStatisticsHub
    {
        Task<List<ChannelMetric>> GetChannelMetrics();
        Task<ImageDownloadServiceStatus> GetDownloadServiceStatus();
        Task<bool> GetIsSystemReady();
        Task<StreamConnectionMetricData> GetStreamConnectionMetricData(GetStreamConnectionMetricDataRequest request);
        Task<List<StreamConnectionMetricData>> GetStreamConnectionMetricDatas();
        Task<SDSystemStatus> GetSystemStatus();
        Task<bool> GetTaskIsRunning();
        Task<VideoInfo> GetVideoInfo(GetVideoInfoRequest request);
        Task<List<VideoInfoDto>> GetVideoInfos();
    }
}
