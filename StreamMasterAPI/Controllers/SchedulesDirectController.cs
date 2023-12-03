using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirectAPI.Domain.JsonClasses;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterApplication.SchedulesDirectAPI;
using StreamMasterApplication.SchedulesDirectAPI.Commands;
using StreamMasterApplication.SchedulesDirectAPI.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterAPI.Controllers;

public class SchedulesDirectController : ApiControllerBase, ISchedulesDirectController
{

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<UserStatus>> GetStatus()
    {
        return await Mediator.Send(new GetStatus()).ConfigureAwait(false);
    }
}