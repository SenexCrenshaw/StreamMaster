using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Logos.Queries;

namespace StreamMaster.Application.Logos
{
    [Authorize]
    public partial class LogosController(ILogger<LogosController> _logger) : ApiControllerBase, ILogosController
    {

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<LogoFileDto>>> GetLogos()
        {
            try
            {
                DataResponse<List<LogoFileDto>> ret = await Sender.Send(new GetLogosRequest()).ConfigureAwait(false);
                return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetLogos.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetLogos.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ILogosHub
    {
        public async Task<List<LogoFileDto>> GetLogos()
        {
            DataResponse<List<LogoFileDto>> ret = await Sender.Send(new GetLogosRequest()).ConfigureAwait(false);
            return ret.Data;
        }

    }
}
