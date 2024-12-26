using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.EPG.Commands;
using StreamMaster.Application.EPG.Queries;

namespace StreamMaster.Application.EPG
{
    [Authorize]
    public partial class EPGController(ILogger<EPGController> _logger) : ApiControllerBase, IEPGController
    {
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<EPGColorDto>>> GetEPGColors()
        {
            try
            {
                var ret = await Sender.Send(new GetEPGColorsRequest()).ConfigureAwait(false);
                return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetEPGColors.", statusCode: 500) : Ok(ret.Data ?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetEPGColors.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> EPGSync()
        {
            var ret = await Sender.Send(new EPGSyncRequest()).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IEPGHub
    {
        public async Task<List<EPGColorDto>> GetEPGColors()
        {
            var ret = await Sender.Send(new GetEPGColorsRequest()).ConfigureAwait(false);
            return ret.Data ?? [];
        }

        public async Task<APIResponse?> EPGSync()
        {
            var ret = await Sender.Send(new EPGSyncRequest()).ConfigureAwait(false);
            return ret;
        }
    }
}