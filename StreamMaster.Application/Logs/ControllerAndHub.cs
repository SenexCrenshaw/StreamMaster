using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Logs.Queries;

namespace StreamMaster.Application.Logs.Controllers
{
    [Authorize]
    public partial class LogsController(ILogger<LogsController> _logger) : ApiControllerBase, ILogsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<string>> GetLogContents([FromQuery] GetLogContentsRequest request)
        {
            try
            {
            DataResponse<string> ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetLogContents.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetLogContents.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<string>>> GetLogNames()
        {
            try
            {
            DataResponse<List<string>> ret = await Sender.Send(new GetLogNamesRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetLogNames.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetLogNames.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ILogsHub
    {
        public async Task<string> GetLogContents(GetLogContentsRequest request)
        {
             DataResponse<string> ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<string>> GetLogNames()
        {
             DataResponse<List<string>> ret = await Sender.Send(new GetLogNamesRequest()).ConfigureAwait(false);
            return ret.Data;
        }

    }
}
