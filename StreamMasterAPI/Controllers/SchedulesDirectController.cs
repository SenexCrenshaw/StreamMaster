using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirectAPI.Domain.JsonClasses;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterApplication.SchedulesDirectAPI;
using StreamMasterApplication.SchedulesDirectAPI.Commands;
using StreamMasterApplication.SchedulesDirectAPI.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterAPI.Controllers;

public class SchedulesDirectController : ApiControllerBase, ISchedulesDirectController
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
    public async Task<ActionResult<List<HeadendDto>>> GetHeadends(string country, string postalCode)
    {
        return Ok(await Mediator.Send(new GetHeadends(country, postalCode)).ConfigureAwait(false));
    }


    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<LineupPreviewChannel>>> GetLineupPreviewChannel(string Lineup)
    {
        return Ok(await Mediator.Send(new GetLineupPreviewChannel(Lineup)).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<SubscribedLineup>>> GetLineups()
    {
        return Ok(await Mediator.Send(new GetLineups()).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<PagedResponse<StationChannelName>>> GetPagedStationChannelNameSelections([FromQuery] StationChannelNameParameters Parameters)
    {
        return Ok(await Mediator.Send(new GetPagedStationChannelNameSelections(Parameters)).ConfigureAwait(false));
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
    public async Task<ActionResult<List<StationChannelName>>> GetStationChannelNamesSimpleQuery([FromQuery] StationChannelNameParameters Parameters)
    {
        return Ok(await Mediator.Send(new GetStationChannelNamesSimpleQuery(Parameters)).ConfigureAwait(false));
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