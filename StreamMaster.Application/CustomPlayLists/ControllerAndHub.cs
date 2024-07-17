using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.CustomPlayLists.Commands;

namespace StreamMaster.Application.CustomPlayLists
{
    public partial class CustomPlayListsController(ILogger<CustomPlayListsController> _logger) : ApiControllerBase, ICustomPlayListsController
    {

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
        public async Task<APIResponse> ScanForCustomPlayLists()
        {
            APIResponse ret = await Sender.Send(new ScanForCustomPlayListsRequest()).ConfigureAwait(false);
            return ret;
        }

    }
}
