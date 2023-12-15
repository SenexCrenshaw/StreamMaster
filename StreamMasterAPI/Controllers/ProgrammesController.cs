using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirectAPI.Domain.XmltvXml;

using StreamMasterApplication.Programmes;
using StreamMasterApplication.Programmes.Queries;

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
    public async Task<ActionResult<IEnumerable<XmltvProgramme>>> GetProgrammes()
    {
        return Ok(await Mediator.Send(new GetProgrammesRequest()).ConfigureAwait(false));
    }

}