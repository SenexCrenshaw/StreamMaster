using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirectAPI.Domain.JsonClasses;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterApplication.SchedulesDirectAPI;
using StreamMasterApplication.SchedulesDirectAPI.Queries;

using StreamMasterDomain.Pagination;

namespace StreamMasterAPI.Controllers;

public class SchedulesDirectController : ApiControllerBase, ISchedulesDirectController
{

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<PagedResponse<StationChannelName>>> GetPagedStationChannelNameSelections([FromQuery] StationChannelNameParameters Parameters)
    {
        return Ok(await Mediator.Send(new GetPagedStationChannelNameSelections(Parameters)).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<StationChannelName>> GetStationChannelNameFromDisplayName(string DisplayName)
    {
        return Ok(await Mediator.Send(new GetStationChannelNameFromDisplayName(DisplayName)).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<StationChannelName>>> GetStationChannelNamesSimpleQuery([FromQuery] StationChannelNameParameters Parameters)
    {
        return Ok(await Mediator.Send(new GetStationChannelNamesSimpleQuery(Parameters)).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<UserStatus>> GetStatus()
    {
        return await Mediator.Send(new GetStatus()).ConfigureAwait(false);
    }
}