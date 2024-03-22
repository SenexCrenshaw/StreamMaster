using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.ChannelGroups.Commands;

namespace StreamMaster.Application.ChannelGroups
{
    public partial class ChannelGroupsController(ISender Sender) : ApiControllerBase
    {        

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<ChannelGroupDto>> CreateChannelGroupRequest(CreateChannelGroupRequest request)
        {
            ChannelGroupDto? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub 
    {
        public async Task<ChannelGroupDto?> CreateChannelGroupRequest(CreateChannelGroupRequest request)
        {
            ChannelGroupDto? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
