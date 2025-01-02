using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.WebSocket.Commands;

namespace StreamMaster.Application.WebSocket.Controllers
{
    [Authorize]
    public partial class WebSocketController() : ApiControllerBase, IWebSocketController
    {
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> TriggerReload()
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(new TriggerReloadRequest())).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IWebSocketHub
    {
        public async Task<APIResponse?> TriggerReload()
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(new TriggerReloadRequest())).ConfigureAwait(false);
            return ret;
        }
    }
}
