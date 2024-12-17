using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.SMTasks.Commands;
using StreamMaster.Application.SMTasks.Queries;

namespace StreamMaster.Application.SMTasks.Controllers
{
    [Authorize]
    public partial class SMTasksController(ILogger<SMTasksController> _logger) : ApiControllerBase, ISMTasksController
    {
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<SMTask>>> GetSMTasks()
        {
            try
            {
            var ret = await Sender.Send(new GetSMTasksRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetSMTasks.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetSMTasks.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SendSMTasks(SendSMTasksRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISMTasksHub
    {
        public async Task<List<SMTask>> GetSMTasks()
        {
             var ret = await Sender.Send(new GetSMTasksRequest()).ConfigureAwait(false);
            return ret.Data?? [];
        }
        public async Task<APIResponse?> SendSMTasks(SendSMTasksRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }
    }
}
