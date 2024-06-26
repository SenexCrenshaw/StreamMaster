using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Icons.CommandsOld;

namespace StreamMaster.API.Controllers;

public class IconsController : ApiControllerBase
{
    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }


    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> ReadDirectoryLogos()
    {
        await Mediator.Send(new ReadDirectoryLogosRequest()).ConfigureAwait(false);
        return Ok();
    }

}