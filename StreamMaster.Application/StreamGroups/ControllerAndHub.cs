using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.StreamGroups.Commands;

namespace StreamMaster.Application.StreamGroups
{
    public partial class StreamGroupsController(ISender Sender) : ApiControllerBase, IStreamGroupsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse<StreamGroupDto>>> GetPagedStreamGroups([FromQuery] QueryStringParameters Parameters)
        {
            APIResponse<StreamGroupDto> ret = await Sender.Send(new GetPagedStreamGroupsRequest(Parameters)).ConfigureAwait(false);
            return ret;
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IStreamGroupsHub
    {
        public async Task<APIResponse<StreamGroupDto>> GetPagedStreamGroups(QueryStringParameters Parameters)
        {
            APIResponse<StreamGroupDto> ret = await Sender.Send(new GetPagedStreamGroupsRequest(Parameters)).ConfigureAwait(false);
            return ret;
        }

    }
}
