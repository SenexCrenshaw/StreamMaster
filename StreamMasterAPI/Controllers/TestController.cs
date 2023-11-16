using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.SchedulesDirectAPI.Commands;

namespace StreamMasterAPI.Controllers;

public class TestController : ApiControllerBase
{
    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult<bool>> SDSync()
    {
        return await Mediator.Send(new SDSync()).ConfigureAwait(false);
    }
}