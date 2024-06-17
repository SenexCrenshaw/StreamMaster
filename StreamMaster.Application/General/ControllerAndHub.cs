using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.General.Commands;

namespace StreamMaster.Application.General.Controllers
{
    public partial class GeneralController(ILogger<GeneralController> _logger) : ApiControllerBase, IGeneralController
    {        

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SetTestTask(SetTestTaskRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IGeneralHub
    {
        public async Task<APIResponse> SetTestTask(SetTestTaskRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
