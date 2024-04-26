using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.General.Commands;
using StreamMaster.Application.SchedulesDirect.Commands;

namespace StreamMaster.API.Controllers;

public class TestController : ApiControllerBase
{
    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult<bool>> EPGSync()
    {
        return await Mediator.Send(new EPGSync()).ConfigureAwait(false);
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> SetSystemReady(SetIsSystemReadyRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }
}