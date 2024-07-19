using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.CustomPlayLists.Commands;
using StreamMaster.Application.CustomPlayLists.Queries;

namespace StreamMaster.Application.CustomPlayLists.Controllers
{
    public partial class CustomPlayListsController(ILogger<CustomPlayListsController> _logger) : ApiControllerBase, ICustomPlayListsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<CustomPlayList>> GetCustomPlayList([FromQuery] GetCustomPlayListRequest request)
        {
            try
            {
            DataResponse<CustomPlayList> ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetCustomPlayList.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetCustomPlayList.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<CustomPlayList>>> GetCustomPlayLists()
        {
            try
            {
            DataResponse<List<CustomPlayList>> ret = await Sender.Send(new GetCustomPlayListsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetCustomPlayLists.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetCustomPlayLists.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> ScanForCustomPlayLists()
        {
            APIResponse ret = await Sender.Send(new ScanForCustomPlayListsRequest()).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ICustomPlayListsHub
    {
        public async Task<CustomPlayList> GetCustomPlayList(GetCustomPlayListRequest request)
        {
             DataResponse<CustomPlayList> ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<CustomPlayList>> GetCustomPlayLists()
        {
             DataResponse<List<CustomPlayList>> ret = await Sender.Send(new GetCustomPlayListsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<APIResponse> ScanForCustomPlayLists()
        {
            APIResponse ret = await Sender.Send(new ScanForCustomPlayListsRequest()).ConfigureAwait(false);
            return ret;
        }

    }
}
