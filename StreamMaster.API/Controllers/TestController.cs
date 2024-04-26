using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.SchedulesDirect.CommandsOld;

namespace StreamMaster.API.Controllers;

public class TestController : ApiControllerBase
{
    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult<bool>> EPGSync()
    {
        return await Mediator.Send(new EPGSync()).ConfigureAwait(false);
    }
}