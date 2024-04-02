using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.M3UFiles.CommandsOrig;
using StreamMaster.Application.M3UFiles.Queries;
using StreamMaster.Domain.API;

namespace StreamMaster.API.Controllers;

public class M3UFilesController() : ApiControllerBase
{


    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult<DefaultAPIResponse>> CreateM3UFileFromForm([FromForm] CreateM3UFileRequest request)
    {
        DefaultAPIResponse result = await Mediator.Send(request).ConfigureAwait(false);
        return result;
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> ChangeM3UFileName(ChangeM3UFileNameRequest request)
    {
        bool result = await Mediator.Send(request).ConfigureAwait(false) != null;
        return result ? Ok() : BadRequest();
    }


    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<M3UFileDto>> GetM3UFile(int id)
    {
        M3UFileDto? data = await Mediator.Send(new GetM3UFileQuery(id)).ConfigureAwait(false);

        return data != null ? (ActionResult<M3UFileDto>)data : (ActionResult<M3UFileDto>)NotFound();
    }


    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> ScanDirectoryForM3UFiles()
    {
        _ = await Mediator.Send(new ScanDirectoryForM3UFilesRequest()).ConfigureAwait(false);
        return NoContent();
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<string>>> GetM3UFileNames()
    {
        List<string> res = await Mediator.Send(new GetM3UFileNamesQuery()).ConfigureAwait(false);

        return Ok(res);
    }
}