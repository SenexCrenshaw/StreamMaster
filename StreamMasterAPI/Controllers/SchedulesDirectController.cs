using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirect;

using StreamMasterApplication.Settings;
using StreamMasterApplication.Settings.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterAPI.Controllers;

public class SchedulesDirectController : ApiControllerBase, ISchedulesDirectController
{
    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<Countries?>> GetCountries()
    {
        return await Mediator.Send(new GetCountries()).ConfigureAwait(false);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<HeadendDto>>> GetHeadends(string country, string postalCode)
    {
        return await Mediator.Send(new GetHeadends(country, postalCode)).ConfigureAwait(false);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<LineUpResult?>> GetLineup(string lineup)
    {
        return await Mediator.Send(new GetLineup(lineup)).ConfigureAwait(false);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<LineUpsResult?>> GetLineups()
    {
        return await Mediator.Send(new GetLineups()).ConfigureAwait(false);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<Schedule>?>> GetSchedules(List<string> stationIds)
    {
        return await Mediator.Send(new GetSchedules(stationIds)).ConfigureAwait(false);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<SDStatus>> GetStatus()
    {
        return await Mediator.Send(new GetStatus()).ConfigureAwait(false);
    }
}
