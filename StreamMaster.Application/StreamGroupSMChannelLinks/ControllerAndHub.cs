using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.StreamGroupSMChannelLinks.Commands;
using StreamMaster.Application.StreamGroupSMChannelLinks.Queries;

namespace StreamMaster.Application.StreamGroupSMChannelLinks.Controllers
{
    [Authorize]
    public partial class StreamGroupSMChannelLinksController(ILogger<StreamGroupSMChannelLinksController> _logger) : ApiControllerBase, IStreamGroupSMChannelLinksController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<SMChannelDto>>> GetStreamGroupSMChannels([FromQuery] GetStreamGroupSMChannelsRequest request)
        {
            try
            {
            DataResponse<List<SMChannelDto>> ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStreamGroupSMChannels.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetStreamGroupSMChannels.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> AddSMChannelsToStreamGroupByParameters(AddSMChannelsToStreamGroupByParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> AddSMChannelsToStreamGroup(AddSMChannelsToStreamGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> AddSMChannelToStreamGroup(AddSMChannelToStreamGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> RemoveSMChannelFromStreamGroup(RemoveSMChannelFromStreamGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IStreamGroupSMChannelLinksHub
    {
        public async Task<List<SMChannelDto>> GetStreamGroupSMChannels(GetStreamGroupSMChannelsRequest request)
        {
             DataResponse<List<SMChannelDto>> ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<APIResponse> AddSMChannelsToStreamGroupByParameters(AddSMChannelsToStreamGroupByParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> AddSMChannelsToStreamGroup(AddSMChannelsToStreamGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> AddSMChannelToStreamGroup(AddSMChannelToStreamGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> RemoveSMChannelFromStreamGroup(RemoveSMChannelFromStreamGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
