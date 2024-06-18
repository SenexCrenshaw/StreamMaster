using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.SchedulesDirect.Queries;

namespace StreamMaster.Application.SchedulesDirect
{
    public partial class SchedulesDirectController(ILogger<SchedulesDirectController> _logger) : ApiControllerBase, ISchedulesDirectController
    {

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<StationChannelName>>> GetStationChannelNames()
        {
            try
            {
                DataResponse<List<StationChannelName>> ret = await Sender.Send(new GetStationChannelNamesRequest()).ConfigureAwait(false);
                return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStationChannelNames.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetStationChannelNames.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISchedulesDirectHub
    {
        public async Task<List<StationChannelName>> GetStationChannelNames()
        {
            DataResponse<List<StationChannelName>> ret = await Sender.Send(new GetStationChannelNamesRequest()).ConfigureAwait(false);
            return ret.Data;
        }

    }
}
