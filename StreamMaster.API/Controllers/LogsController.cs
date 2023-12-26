using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Dto;

using StreamMaster.Application.LogApp;
using StreamMaster.Application.LogApp.Queries;

namespace StreamMasterAPI.Controllers;

public class LogsController : ApiControllerBase, ILogController
{
    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<LogEntryDto>>> GetLog([FromQuery] GetLogRequest request)
    {
        return await Mediator.Send(request).ConfigureAwait(false);
    }
}
