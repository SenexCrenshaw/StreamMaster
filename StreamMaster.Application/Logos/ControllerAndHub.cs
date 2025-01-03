using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Logos.Commands;
using StreamMaster.Application.Logos.Queries;

namespace StreamMaster.Application.Logos.Controllers
{
    [Authorize]
    public partial class LogosController(ILogger<LogosController> _logger) : ApiControllerBase, ILogosController
    {
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<CustomLogoDto>>> GetCustomLogos()
        {
            try
            {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetCustomLogosRequest())).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetCustomLogos.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetCustomLogos.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<LogoDto>> GetLogoForChannel([FromQuery] GetLogoForChannelRequest request)
        {
            try
            {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
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
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
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
        public async Task<ActionResult<List<CustomLogoDto>>> GetLogos()
        {
            try
            {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetLogosRequest())).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetLogos.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetLogos.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> AddCustomLogo(AddCustomLogoRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> RemoveCustomLogo(RemoveCustomLogoRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ILogosHub
    {
        public async Task<List<CustomLogoDto>> GetCustomLogos()
        {
             var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetCustomLogosRequest())).ConfigureAwait(false);
            return ret.Data?? [];
        }
        public async Task<LogoDto> GetLogoForChannel(GetLogoForChannelRequest request)
        {
             var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret.Data?? new();
        }
        public async Task<LogoDto> GetLogo(GetLogoRequest request)
        {
             var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret.Data?? new();
        }
        public async Task<List<CustomLogoDto>> GetLogos()
        {
             var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetLogosRequest())).ConfigureAwait(false);
            return ret.Data?? [];
        }
        public async Task<APIResponse?> AddCustomLogo(AddCustomLogoRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> RemoveCustomLogo(RemoveCustomLogoRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret;
        }
    }
}
