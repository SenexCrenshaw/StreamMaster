using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Statistics.Queries;

namespace StreamMaster.Application.Statistics.Controllers
{
    public partial class StatisticsController(ILogger<StatisticsController> _logger) : ApiControllerBase, IStatisticsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<ChannelDistributorDto>>> GetChannelDistributors()
        {
            try
            {
            DataResponse<List<ChannelDistributorDto>> ret = await Sender.Send(new GetChannelDistributorsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetChannelDistributors.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetChannelDistributors.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<ChannelStreamingStatistics>>> GetChannelStreamingStatistics()
        {
            try
            {
            DataResponse<List<ChannelStreamingStatistics>> ret = await Sender.Send(new GetChannelStreamingStatisticsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetChannelStreamingStatistics.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetChannelStreamingStatistics.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<ClientStreamingStatistics>>> GetClientStreamingStatistics()
        {
            try
            {
            DataResponse<List<ClientStreamingStatistics>> ret = await Sender.Send(new GetClientStreamingStatisticsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetClientStreamingStatistics.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetClientStreamingStatistics.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<StreamStreamingStatistic>>> GetStreamingStatisticsForChannel([FromQuery] GetStreamingStatisticsForChannelRequest request)
        {
            try
            {
            DataResponse<List<StreamStreamingStatistic>> ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStreamingStatisticsForChannel.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetStreamingStatisticsForChannel.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<StreamStreamingStatistic>>> GetStreamStreamingStatistics()
        {
            try
            {
            DataResponse<List<StreamStreamingStatistic>> ret = await Sender.Send(new GetStreamStreamingStatisticsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStreamStreamingStatistics.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetStreamStreamingStatistics.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IStatisticsHub
    {
        public async Task<List<ChannelDistributorDto>> GetChannelDistributors()
        {
             DataResponse<List<ChannelDistributorDto>> ret = await Sender.Send(new GetChannelDistributorsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<ChannelStreamingStatistics>> GetChannelStreamingStatistics()
        {
             DataResponse<List<ChannelStreamingStatistics>> ret = await Sender.Send(new GetChannelStreamingStatisticsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<ClientStreamingStatistics>> GetClientStreamingStatistics()
        {
             DataResponse<List<ClientStreamingStatistics>> ret = await Sender.Send(new GetClientStreamingStatisticsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<StreamStreamingStatistic>> GetStreamingStatisticsForChannel(GetStreamingStatisticsForChannelRequest request)
        {
             DataResponse<List<StreamStreamingStatistic>> ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<StreamStreamingStatistic>> GetStreamStreamingStatistics()
        {
             DataResponse<List<StreamStreamingStatistic>> ret = await Sender.Send(new GetStreamStreamingStatisticsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

    }
}
