using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Custom.Commands;
using StreamMaster.Application.Custom.Queries;

namespace StreamMaster.Application.Custom.Controllers
{
    [Authorize]
    public partial class CustomController(ILogger<CustomController> _logger) : ApiControllerBase, ICustomController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<CustomPlayList>> GetCustomPlayList([FromQuery] GetCustomPlayListRequest request)
        {
            try
            {
            var ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetCustomPlayList.", statusCode: 500) : Ok(ret.Data?? new());
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
            var ret = await Sender.Send(new GetCustomPlayListsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetCustomPlayLists.", statusCode: 500) : Ok(ret.Data?? new());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetCustomPlayLists.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<CustomPlayList>>> GetIntroPlayLists()
        {
            try
            {
            var ret = await Sender.Send(new GetIntroPlayListsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetIntroPlayLists.", statusCode: 500) : Ok(ret.Data?? new());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetIntroPlayLists.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> ScanForCustom()
        {
            var ret = await Sender.Send(new ScanForCustomRequest()).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ICustomHub
    {
        public async Task<CustomPlayList> GetCustomPlayList(GetCustomPlayListRequest request)
        {
             var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data?? new();
        }

        public async Task<List<CustomPlayList>> GetCustomPlayLists()
        {
             var ret = await Sender.Send(new GetCustomPlayListsRequest()).ConfigureAwait(false);
            return ret.Data?? new();
        }

        public async Task<List<CustomPlayList>> GetIntroPlayLists()
        {
             var ret = await Sender.Send(new GetIntroPlayListsRequest()).ConfigureAwait(false);
            return ret.Data?? new();
        }

        public async Task<APIResponse?> ScanForCustom()
        {
            var ret = await Sender.Send(new ScanForCustomRequest()).ConfigureAwait(false);
            return ret;
        }

    }
}
