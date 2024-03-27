using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMStreams.Commands;

namespace StreamMaster.Application.SMStreams
{
    public partial class SMStreamsController(ISender Sender) : ApiControllerBase, ISMStreamsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse<SMStreamDto>>> GetPagedSMStreams([FromQuery] SMStreamParameters Parameters)
        {
            APIResponse<SMStreamDto> ret = await Sender.Send(new GetPagedSMStreams(Parameters)).ConfigureAwait(false);
            return ret;
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> ToggleSMStreamVisibleById(ToggleSMStreamVisibleByIdRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISMStreamsHub
    {
        public async Task<APIResponse<SMStreamDto>> GetPagedSMStreams(SMStreamParameters Parameters)
        {
            APIResponse<SMStreamDto> ret = await Sender.Send(new GetPagedSMStreams(Parameters)).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> ToggleSMStreamVisibleById(ToggleSMStreamVisibleByIdRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
