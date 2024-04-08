using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMStreams.Commands;
using StreamMaster.Application.SMStreams.Queries;

namespace StreamMaster.Application.SMStreams.Controllers
{
    public partial class SMStreamsController(ILogger<SMStreamsController> _logger) : ApiControllerBase, ISMStreamsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<PagedResponse<SMStreamDto>>> GetPagedSMStreams([FromQuery] QueryStringParameters Parameters)
        {
            PagedResponse<SMStreamDto> ret = await Sender.Send(new GetPagedSMStreamsRequest(Parameters)).ConfigureAwait(false);
            return ret;
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> ToggleSMStreamVisibleById(ToggleSMStreamVisibleByIdRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISMStreamsHub
    {
        public async Task<PagedResponse<SMStreamDto>> GetPagedSMStreams(QueryStringParameters Parameters)
        {
            PagedResponse<SMStreamDto> ret = await Sender.Send(new GetPagedSMStreamsRequest(Parameters)).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> ToggleSMStreamVisibleById(ToggleSMStreamVisibleByIdRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
