using MediatR;

using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.LogApp;
using StreamMasterApplication.LogApp.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterAPI.Controllers;

public class LogsController : ApiControllerBase, ILogController
{
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<LogEntryDto>>> GetLogRequest(GetLog request)
    {
        return await Mediator.Send(request).ConfigureAwait(false);
    }
}
