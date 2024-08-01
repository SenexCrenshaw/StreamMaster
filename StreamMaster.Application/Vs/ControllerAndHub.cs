using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Vs.Queries;

namespace StreamMaster.Application.Vs.Controllers
{
    public partial class VsController(ILogger<VsController> _logger) : ApiControllerBase, IVsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<V>>> GetVs([FromQuery] GetVsRequest request)
        {
            try
            {
            DataResponse<List<V>> ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetVs.", statusCode: 500) : Ok(ret.Data);
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
             DataResponse<List<V>> ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data;
        }

    }
}
