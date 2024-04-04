using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.M3UFiles.CommandsOrig;
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


    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> ScanDirectoryForM3UFiles()
    {
        _ = await Mediator.Send(new ScanDirectoryForM3UFilesRequest()).ConfigureAwait(false);
        return NoContent();
    }

}