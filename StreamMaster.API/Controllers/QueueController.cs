using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Queue;
using StreamMaster.Application.Queue.Queries;

namespace StreamMaster.API.Controllers;

public class QueueController : ApiControllerBase, IQueueController
{
    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<SMTask>>> GetQueueStatus()
    {
        return await Mediator.Send(new GetQueueStatus()).ConfigureAwait(false);
    }
}