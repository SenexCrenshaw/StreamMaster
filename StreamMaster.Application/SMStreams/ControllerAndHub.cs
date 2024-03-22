using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMStreams.Commands;

namespace StreamMaster.Application.SMStreams
{
    public partial class SMStreamsController(ISender Sender) : ApiControllerBase
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse<SMStreamDto>>> GetPagedSMStreams([FromQuery] SMStreamParameters Parameters)
        {
            APIResponse<SMStreamDto> ret = await Sender.Send(new GetPagedSMStreams(Parameters)).ConfigureAwait(false);
            return ret;
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> ToggleSMStreamVisibleById(ToggleSMStreamVisibleById request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub 
    {
        public async Task<APIResponse<SMStreamDto>?> GetPagedSMStreams(GetPagedSMStreams request)
        {
            APIResponse<SMStreamDto>? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse?> ToggleSMStreamVisibleById(ToggleSMStreamVisibleById request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
