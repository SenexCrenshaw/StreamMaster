using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMMessages.Commands;

namespace StreamMaster.Application.SMMessages
{
    public partial class SMMessagesController(ISender Sender, ILogger<SMMessagesController> _logger) : ApiControllerBase, ISMMessagesController
    {        

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> SendSMError(SendSMErrorRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> SendSMInfo(SendSMInfoRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> SendSMMessage(SendSMMessageRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> SendSMWarn(SendSMWarnRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> SendSuccess(SendSuccessRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISMMessagesHub
    {
        public async Task<DefaultAPIResponse> SendSMError(SendSMErrorRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> SendSMInfo(SendSMInfoRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> SendSMMessage(SendSMMessageRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> SendSMWarn(SendSMWarnRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> SendSuccess(SendSuccessRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
