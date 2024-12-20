using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.SMMessages.Commands;

namespace StreamMaster.Application.SMMessages.Controllers
{
    [Authorize]
    public partial class SMMessagesController() : ApiControllerBase, ISMMessagesController
    {
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SendSMError(SendSMErrorRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SendSMInfo(SendSMInfoRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SendSMMessage(SendSMMessageRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SendSMWarn(SendSMWarnRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SendSuccess(SendSuccessRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISMMessagesHub
    {
        public async Task<APIResponse?> SendSMError(SendSMErrorRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> SendSMInfo(SendSMInfoRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> SendSMMessage(SendSMMessageRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> SendSMWarn(SendSMWarnRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> SendSuccess(SendSuccessRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }
    }
}
