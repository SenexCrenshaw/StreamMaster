using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Statistics.Queries;

namespace StreamMaster.Application.Statistics.Controllers
{
    public partial class StatisticsController(ILogger<StatisticsController> _logger) : ApiControllerBase, IStatisticsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<ChannelMetric>>> GetChannelMetrics()
        {
            try
            {
            DataResponse<List<ChannelMetric>> ret = await Sender.Send(new GetChannelMetricsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetChannelMetrics.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetChannelMetrics.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<VideoInfo>> GetVideoInfo([FromQuery] GetVideoInfoRequest request)
        {
            try
            {
            DataResponse<VideoInfo> ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetVideoInfo.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetVideoInfo.");
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
            return ret.Data;
        }

        public async Task<VideoInfo> GetVideoInfo(GetVideoInfoRequest request)
        {
             DataResponse<VideoInfo> ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data;
        }

    }
}
