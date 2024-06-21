using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Statistics.Queries;

namespace StreamMaster.Application.Statistics.Controllers
{
    public partial class StatisticsController(ILogger<StatisticsController> _logger) : ApiControllerBase, IStatisticsController
    {        

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
        public async Task<ActionResult<List<InputStreamingStatistics>>> GetInputStatistics()
        {
            try
            {
            DataResponse<List<InputStreamingStatistics>> ret = await Sender.Send(new GetInputStatisticsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetInputStatistics.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetInputStatistics.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IStatisticsHub
    {
        public async Task<List<ClientStreamingStatistics>> GetClientStreamingStatistics()
        {
             DataResponse<List<ClientStreamingStatistics>> ret = await Sender.Send(new GetClientStreamingStatisticsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<InputStreamingStatistics>> GetInputStatistics()
        {
             DataResponse<List<InputStreamingStatistics>> ret = await Sender.Send(new GetInputStatisticsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

    }
}
