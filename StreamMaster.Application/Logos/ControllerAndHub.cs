using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Logos.Queries;

namespace StreamMaster.Application.Logos.Controllers
{
    [Authorize]
    public partial class LogosController(ILogger<LogosController> _logger) : ApiControllerBase, ILogosController
    {
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<LogoDto>> GetLogoForChannel([FromQuery] GetLogoForChannelRequest request)
        {
            try
            {
            var ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetLogoForChannel.", statusCode: 500) : Ok(ret.Data?? new());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetLogoForChannel.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<LogoDto>> GetLogo([FromQuery] GetLogoRequest request)
        {
            try
            {
            var ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetLogo.", statusCode: 500) : Ok(ret.Data?? new());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetLogo.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<LogoFileDto>>> GetLogos()
        {
            try
            {
            var ret = await Sender.Send(new GetLogosRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetLogos.", statusCode: 500) : Ok(ret.Data?? []);
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
        public async Task<LogoDto> GetLogoForChannel(GetLogoForChannelRequest request)
        {
             var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data?? new();
        }
        public async Task<LogoDto> GetLogo(GetLogoRequest request)
        {
             var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data?? new();
        }
        public async Task<List<LogoFileDto>> GetLogos()
        {
             var ret = await Sender.Send(new GetLogosRequest()).ConfigureAwait(false);
            return ret.Data?? [];
        }
    }
}
