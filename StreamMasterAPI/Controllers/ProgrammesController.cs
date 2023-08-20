using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Programmes;
using StreamMasterApplication.Programmes.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterAPI.Controllers;

public class ProgrammesController : ApiControllerBase, IProgrammeChannelController
{
    [HttpGet]
    [Route("GetProgramme/{channel}")]
    public async Task<ActionResult<IEnumerable<Programme>?>> GetProgramme(string channel)
    {
        IEnumerable<Programme>? data = await Mediator.Send(new GetProgramme(channel)).ConfigureAwait(false);
        return data is not null ? (ActionResult<IEnumerable<Programme>?>)Ok(data.ToList()) : (ActionResult<IEnumerable<Programme>?>)NotFound();
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<ProgrammeChannel>>> GetProgrammeChannels()
    {
        return Ok(await Mediator.Send(new GetProgrammeChannels()).ConfigureAwait(false));
    }


    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<PagedResponse<ProgrammeNameDto>>> GetProgrammeNameSelections([FromQuery] ProgrammeParameters Parameters)
    {
        return Ok(await Mediator.Send(new GetProgrammeNameSelections(Parameters)).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<Programme>>> GetProgrammes()
    {
        return Ok(await Mediator.Send(new GetProgrammes()).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<string>>> GetProgrammeNames()
    {
        return Ok(await Mediator.Send(new GetProgrammeNames()).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<ProgrammeNameDto>>> GetProgrammsSimpleQuery([FromQuery] ProgrammeParameters Parameters)
    {
        return Ok(await Mediator.Send(new GetProgrammsSimpleQuery(Parameters)).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<ProgrammeNameDto?>> GetProgrammeFromDisplayName(string Tvg_ID)
    {
        return Ok(await Mediator.Send(new GetProgrammeFromDisplayName(Tvg_ID)).ConfigureAwait(false));
    }
}