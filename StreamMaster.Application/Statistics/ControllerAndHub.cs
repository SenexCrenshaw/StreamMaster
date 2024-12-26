using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Statistics.Queries;

namespace StreamMaster.Application.Statistics.Controllers
{
    [Authorize]
    public partial class StatisticsController(ILogger<StatisticsController> _logger) : ApiControllerBase, IStatisticsController
    {
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<ChannelMetric>>> GetChannelMetrics()
        {
            try
            {
                DataResponse<List<ChannelMetric>> ret = await Sender.Send(new GetChannelMetricsRequest()).ConfigureAwait(false);
                return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetChannelMetrics.", statusCode: 500) : Ok(ret.Data ?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetChannelMetrics.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<StreamConnectionMetric>> GetStreamConnectionMetric([FromQuery] GetStreamConnectionMetricRequest request)
        {
            try
            {
                DataResponse<StreamConnectionMetric> ret = await Sender.Send(request).ConfigureAwait(false);
                return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStreamConnectionMetric.", statusCode: 500) : Ok(ret.Data ?? new());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetStreamConnectionMetric.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<StreamConnectionMetric>>> GetStreamConnectionMetrics()
        {
            try
            {
                DataResponse<List<StreamConnectionMetric>> ret = await Sender.Send(new GetStreamConnectionMetricsRequest()).ConfigureAwait(false);
                return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStreamConnectionMetrics.", statusCode: 500) : Ok(ret.Data ?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetStreamConnectionMetrics.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<VideoInfo>> GetVideoInfo([FromQuery] GetVideoInfoRequest request)
        {
            try
            {
                DataResponse<VideoInfo?> ret = await Sender.Send(request).ConfigureAwait(false);
                return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetVideoInfo.", statusCode: 500) : Ok(ret.Data ?? new());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetVideoInfo.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<VideoInfoDto>>> GetVideoInfos()
        {
            try
            {
                DataResponse<List<VideoInfoDto>> ret = await Sender.Send(new GetVideoInfosRequest()).ConfigureAwait(false);
                return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetVideoInfos.", statusCode: 500) : Ok(ret.Data ?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetVideoInfos.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IStatisticsHub
    {
        public async Task<List<ChannelMetric>> GetChannelMetrics()
        {
            DataResponse<List<ChannelMetric>> ret = await Sender.Send(new GetChannelMetricsRequest()).ConfigureAwait(false);
            return ret.Data ?? [];
        }
        public async Task<StreamConnectionMetric> GetStreamConnectionMetric(GetStreamConnectionMetricRequest request)
        {
            DataResponse<StreamConnectionMetric> ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data ?? new();
        }
        public async Task<List<StreamConnectionMetric>> GetStreamConnectionMetrics()
        {
            DataResponse<List<StreamConnectionMetric>> ret = await Sender.Send(new GetStreamConnectionMetricsRequest()).ConfigureAwait(false);
            return ret.Data ?? [];
        }
        public async Task<VideoInfo> GetVideoInfo(GetVideoInfoRequest request)
        {
            DataResponse<VideoInfo?> ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data ?? new();
        }
        public async Task<List<VideoInfoDto>> GetVideoInfos()
        {
            DataResponse<List<VideoInfoDto>> ret = await Sender.Send(new GetVideoInfosRequest()).ConfigureAwait(false);
            return ret.Data ?? [];
        }
    }
}
