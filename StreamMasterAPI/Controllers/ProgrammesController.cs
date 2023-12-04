using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirectAPI.Domain.XmltvXml;

using StreamMasterApplication.Programmes;
using StreamMasterApplication.Programmes.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Models;
using StreamMasterDomain.Pagination;

namespace StreamMasterAPI.Controllers;

public class ProgrammesController : ApiControllerBase, IProgrammeChannelController
{
    [HttpGet]
    [Route("GetProgramme/{channel}")]
    public async Task<ActionResult<IEnumerable<XmltvProgramme>?>> GetProgramme(string channel)
    {
        IEnumerable<XmltvProgramme>? data = await Mediator.Send(new GetProgramme(channel)).ConfigureAwait(false);
        return data is not null ? (ActionResult<IEnumerable<XmltvProgramme>?>)Ok(data.ToList()) : (ActionResult<IEnumerable<XmltvProgramme>?>)NotFound();
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<ProgrammeChannel>>> GetProgrammeChannels()
    {
        return Ok(await Mediator.Send(new GetProgrammeChannels()).ConfigureAwait(false));
    }


    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<PagedResponse<ProgrammeNameDto>>> GetPagedProgrammeNameSelections([FromQuery] ProgrammeParameters Parameters)
    {
        return Ok(await Mediator.Send(new GetPagedProgrammeNameSelections(Parameters)).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<XmltvProgramme>>> GetProgrammes()
    {
        return Ok(await Mediator.Send(new GetProgrammesRequest()).ConfigureAwait(false));
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
    public async Task<ActionResult<ProgrammeNameDto?>> GetProgrammeFromDisplayName(GetProgrammeFromDisplayNameRequest request)
    {
        return Ok(await Mediator.Send(request).ConfigureAwait(false));
    }
}