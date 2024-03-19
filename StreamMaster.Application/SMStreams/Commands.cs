using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMStreams;

namespace StreamMaster.Application.SMStreams
{
    public partial class SMStreamsController(ISMStreamsService SMStreamsService) : ApiControllerBase, ISMStreamsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse<SMStreamDto>>> GetPagedSMStreams([FromQuery] SMStreamParameters Parameters)
        {
            APIResponse<SMStreamDto> ret = await SMStreamsService.GetPagedSMStreams(Parameters).ConfigureAwait(false);
            return ret.IsError.HasValue && ret.IsError.Value ? NotFound(ret) : Ok(ret);
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> ToggleSMStreamVisibleById(string id)
        {
            DefaultAPIResponse ret = await SMStreamsService.ToggleSMStreamVisibleById(id).ConfigureAwait(false);
            return ret.IsError.HasValue && ret.IsError.Value ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISMStreamHub
    {
        public async Task<APIResponse<SMStreamDto>> GetPagedSMStreams(SMStreamParameters Parameters)
        {
            APIResponse<SMStreamDto> ret = await SMStreamsService.GetPagedSMStreams(Parameters).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> ToggleSMStreamVisibleById(string id)
        {
            DefaultAPIResponse ret = await SMStreamsService.ToggleSMStreamVisibleById(id).ConfigureAwait(false);
            return ret;
        }

    }
}
