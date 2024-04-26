using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.SchedulesDirect.CommandsOld;
using StreamMaster.Application.SchedulesDirect.QueriesOld;
using StreamMaster.SchedulesDirect.Domain.Dto;

namespace StreamMaster.API.Controllers;

public class SchedulesDirectController : ApiControllerBase
{
    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult<bool>> AddLineup(AddLineup request)
    {
        return Ok(await Mediator.Send(request).ConfigureAwait(false));
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult<bool>> AddStation(AddStation request)
    {
        return Ok(await Mediator.Send(request).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<CountryData>?>> GetAvailableCountries()
    {
        return Ok(await Mediator.Send(new GetAvailableCountries()).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<string>>> GetChannelNames()
    {
        return Ok(await Mediator.Send(new GetChannelNames()).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<HeadendDto>>> GetHeadends(GetHeadends request)
    {
        return Ok(await Mediator.Send(request).ConfigureAwait(false));
    }


    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<LineupPreviewChannel>>> GetLineupPreviewChannel(GetLineupPreviewChannel request)
    {
        return Ok(await Mediator.Send(request).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<SubscribedLineup>>> GetLineups()
    {
        return Ok(await Mediator.Send(new GetLineups()).ConfigureAwait(false));
    }


    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<StationIdLineup>>> GetSelectedStationIds()
    {
        return Ok(await Mediator.Send(new GetSelectedStationIds()).ConfigureAwait(false));
    }
    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<StationChannelMap>>> GetStationChannelMaps()
    {
        return Ok(await Mediator.Send(new GetStationChannelMaps()).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<StationChannelName>> GetStationChannelNameFromDisplayName(GetStationChannelNameFromDisplayName request)
    {
        return Ok(await Mediator.Send(request).ConfigureAwait(false));
    }


    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<StationPreview>>> GetStationPreviews()
    {
        return Ok(await Mediator.Send(new GetStationPreviews()).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<UserStatus>> GetUserStatus()
    {
        return await Mediator.Send(new GetUserStatus()).ConfigureAwait(false);
    }
    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult<bool>> RemoveLineup(RemoveLineup request)
    {
        return Ok(await Mediator.Send(request).ConfigureAwait(false));
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult<bool>> RemoveStation(RemoveStation request)
    {
        return Ok(await Mediator.Send(request).ConfigureAwait(false));
    }
}