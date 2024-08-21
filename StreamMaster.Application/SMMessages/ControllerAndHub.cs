using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMMessages.Commands;

namespace StreamMaster.Application.SMMessages.Controllers
{
    public partial class SMMessagesController() : ApiControllerBase, ISMMessagesController
    {        

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SendSMError(SendSMErrorRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SendSMInfo(SendSMInfoRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SendSMMessage(SendSMMessageRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SendSMWarn(SendSMWarnRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SendSuccess(SendSuccessRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISMMessagesHub
    {
        public async Task<APIResponse> SendSMError(SendSMErrorRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SendSMInfo(SendSMInfoRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SendSMMessage(SendSMMessageRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SendSMWarn(SendSMWarnRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SendSuccess(SendSuccessRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
