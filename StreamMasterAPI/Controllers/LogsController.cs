using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.LogApp;
using StreamMasterApplication.LogApp.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterAPI.Controllers;

public class LogsController : ApiControllerBase, ILogController
{
    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<LogEntryDto>>> GetLogRequest([FromQuery] GetLog request)
    {
        return await Mediator.Send(request).ConfigureAwait(false);
    }
}
