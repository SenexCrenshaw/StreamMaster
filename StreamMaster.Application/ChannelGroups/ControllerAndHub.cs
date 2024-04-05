using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.ChannelGroups.Commands;
using StreamMaster.Application.ChannelGroups.Queries;

namespace StreamMaster.Application.ChannelGroups
{
    public partial class ChannelGroupsController(ISender Sender, ILogger<ChannelGroupsController> _logger) : ApiControllerBase, IChannelGroupsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<PagedResponse<ChannelGroupDto>>> GetPagedChannelGroups([FromQuery] QueryStringParameters Parameters)
        {
            PagedResponse<ChannelGroupDto> ret = await Sender.Send(new GetPagedChannelGroupsRequest(Parameters)).ConfigureAwait(false);
            return ret;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> CreateChannelGroup(CreateChannelGroupRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> DeleteAllChannelGroupsFromParameters(DeleteAllChannelGroupsFromParametersRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> DeleteChannelGroup(DeleteChannelGroupRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> UpdateChannelGroup(UpdateChannelGroupRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IChannelGroupsHub
    {
        public async Task<PagedResponse<ChannelGroupDto>> GetPagedChannelGroups(QueryStringParameters Parameters)
        {
            PagedResponse<ChannelGroupDto> ret = await Sender.Send(new GetPagedChannelGroupsRequest(Parameters)).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> CreateChannelGroup(CreateChannelGroupRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> DeleteAllChannelGroupsFromParameters(DeleteAllChannelGroupsFromParametersRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> DeleteChannelGroup(DeleteChannelGroupRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> UpdateChannelGroup(UpdateChannelGroupRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
