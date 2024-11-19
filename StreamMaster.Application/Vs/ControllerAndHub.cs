using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Vs.Queries;

namespace StreamMaster.Application.Vs.Controllers
{
    [Authorize]
    public partial class VsController(ILogger<VsController> _logger) : ApiControllerBase, IVsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<V>>> GetVs([FromQuery] GetVsRequest request)
        {
            try
            {
            var ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetVs.", statusCode: 500) : Ok(ret.Data?? new());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetVs.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IVsHub
    {
        public async Task<List<V>> GetVs(GetVsRequest request)
        {
             var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data?? new();
        }

    }
}
