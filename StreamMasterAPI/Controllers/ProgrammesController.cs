using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Programmes;
using StreamMasterApplication.Programmes.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterAPI.Controllers;

public class ProgrammesController : ApiControllerBase, IProgrammeChannelController
{
    [HttpGet]
    [Route("GetProgramme/{channel}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Programme>))]
    public async Task<ActionResult<IEnumerable<Programme>?>> GetProgramme(string channel)
    {
        IEnumerable<Programme>? data = await Mediator.Send(new GetProgramme(channel)).ConfigureAwait(false);
        return data is not null ? (ActionResult<IEnumerable<Programme>?>)Ok(data.ToList()) : (ActionResult<IEnumerable<Programme>?>)NotFound();
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProgrammeChannel>))]
    public async Task<ActionResult<IEnumerable<ProgrammeChannel>>> GetProgrammeChannels()
    {
        return Ok(await Mediator.Send(new GetProgrammeChannels()).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProgrammeNameDto>))]
    public async Task<ActionResult<IEnumerable<ProgrammeNameDto>>> GetProgrammeNames()
    {
        return Ok(await Mediator.Send(new GetProgrammeNames()).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Programme>))]
    public async Task<ActionResult<IEnumerable<Programme>>> GetProgrammes()
    {
        return Ok(await Mediator.Send(new GetProgrammes()).ConfigureAwait(false));
    }
}
