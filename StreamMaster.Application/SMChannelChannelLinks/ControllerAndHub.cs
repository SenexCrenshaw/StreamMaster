using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.SMChannelChannelLinks.Commands;
using StreamMaster.Application.SMChannelChannelLinks.Queries;

namespace StreamMaster.Application.SMChannelChannelLinks.Controllers
{
    [Authorize]
    public partial class SMChannelChannelLinksController(ILogger<SMChannelChannelLinksController> _logger) : ApiControllerBase, ISMChannelChannelLinksController
    {
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<SMChannelDto>>> GetSMChannelChannels([FromQuery] GetSMChannelChannelsRequest request)
        {
            try
            {
            var ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetSMChannelChannels.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetSMChannelChannels.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> AddSMChannelToSMChannel(AddSMChannelToSMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> RemoveSMChannelFromSMChannel(RemoveSMChannelFromSMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetSMChannelRanks(SetSMChannelRanksRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISMChannelChannelLinksHub
    {
        public async Task<List<SMChannelDto>> GetSMChannelChannels(GetSMChannelChannelsRequest request)
        {
             var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data?? [];
        }
        public async Task<APIResponse?> AddSMChannelToSMChannel(AddSMChannelToSMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> RemoveSMChannelFromSMChannel(RemoveSMChannelFromSMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> SetSMChannelRanks(SetSMChannelRanksRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }
    }
}
