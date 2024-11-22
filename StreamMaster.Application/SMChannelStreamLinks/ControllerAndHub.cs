using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.SMChannelStreamLinks.Commands;
using StreamMaster.Application.SMChannelStreamLinks.Queries;

namespace StreamMaster.Application.SMChannelStreamLinks.Controllers
{
    [Authorize]
    public partial class SMChannelStreamLinksController(ILogger<SMChannelStreamLinksController> _logger) : ApiControllerBase, ISMChannelStreamLinksController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<SMStreamDto>>> GetSMChannelStreams([FromQuery] GetSMChannelStreamsRequest request)
        {
            try
            {
            var ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetSMChannelStreams.", statusCode: 500) : Ok(ret.Data?? new());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetSMChannelStreams.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> AddSMStreamToSMChannel(AddSMStreamToSMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> RemoveSMStreamFromSMChannel(RemoveSMStreamFromSMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetSMStreamRanks(SetSMStreamRanksRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISMChannelStreamLinksHub
    {
        public async Task<List<SMStreamDto>> GetSMChannelStreams(GetSMChannelStreamsRequest request)
        {
             var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data?? new();
        }

        public async Task<APIResponse?> AddSMStreamToSMChannel(AddSMStreamToSMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> RemoveSMStreamFromSMChannel(RemoveSMStreamFromSMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> SetSMStreamRanks(SetSMStreamRanksRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
