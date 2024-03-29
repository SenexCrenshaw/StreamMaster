using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Icons.Commands;
using StreamMaster.Application.Icons.Queries;
using StreamMaster.Domain.Pagination;

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
    public async Task<ActionResult<IconFileDto>> GetIconFromSource([FromQuery] GetIconFromSourceRequest request)
    {
        IconFileDto? data = await Mediator.Send(request).ConfigureAwait(false);
        return data != null ? (ActionResult<IconFileDto>)data : (ActionResult<IconFileDto>)NotFound();
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> ReadDirectoryLogos()
    {
        await Mediator.Send(new ReadDirectoryLogosRequest()).ConfigureAwait(false);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<IconFileDto>>> GetPagedIcons([FromQuery] IconFileParameters iconFileParameters)
    {
        PagedResponse<IconFileDto> result = await Mediator.Send(new GetPagedIcons(iconFileParameters)).ConfigureAwait(false);

        return Ok(result);
    }
    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<IconFileDto>>> GetIconsSimpleQuery([FromQuery] IconFileParameters iconFileParameters)
    {
        IEnumerable<IconFileDto> result = await Mediator.Send(new GetIconsSimpleQuery(iconFileParameters)).ConfigureAwait(false);
        return Ok(result);
    }

}