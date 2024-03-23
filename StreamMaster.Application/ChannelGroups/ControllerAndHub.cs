using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.ChannelGroups.Commands;

namespace StreamMaster.Application.ChannelGroups
{
    public partial class ChannelGroupsController(ISender Sender) : ApiControllerBase, IChannelGroupsController
    {        

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> CreateChannelGroup(CreateChannelGroupRequest request)
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
        public async Task<DefaultAPIResponse> CreateChannelGroup(CreateChannelGroupRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
