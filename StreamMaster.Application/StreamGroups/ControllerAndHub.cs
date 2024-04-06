using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.StreamGroups.Queries;

namespace StreamMaster.Application.StreamGroups.Controllers
{
    public partial class StreamGroupsController(ISender Sender, ILogger<StreamGroupsController> _logger) : ApiControllerBase, IStreamGroupsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<PagedResponse<StreamGroupDto>>> GetPagedStreamGroups([FromQuery] QueryStringParameters Parameters)
        {
            PagedResponse<StreamGroupDto> ret = await Sender.Send(new GetPagedStreamGroupsRequest(Parameters)).ConfigureAwait(false);
            return ret;
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IStreamGroupsHub
    {
        public async Task<PagedResponse<StreamGroupDto>> GetPagedStreamGroups(QueryStringParameters Parameters)
        {
            PagedResponse<StreamGroupDto> ret = await Sender.Send(new GetPagedStreamGroupsRequest(Parameters)).ConfigureAwait(false);
            return ret;
        }

    }
}
