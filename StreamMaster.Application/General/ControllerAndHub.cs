using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.General.Commands;

namespace StreamMaster.Application.General.Controllers
{
    [Authorize]
    public partial class GeneralController() : ApiControllerBase, IGeneralController
    {
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetTestTask(SetTestTaskRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IGeneralHub
    {
        public async Task<APIResponse?> SetTestTask(SetTestTaskRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret;
        }
    }
}
