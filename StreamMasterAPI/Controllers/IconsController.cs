using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Icons;
using StreamMasterApplication.Icons.Commands;
using StreamMasterApplication.Icons.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterAPI.Controllers;

public class IconsController : ApiControllerBase, IIconController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IconFileDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddIconFile(AddIconFileRequest request)
    {
        IconFileDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? Ok() : CreatedAtAction(nameof(GetIcon), new { id = entity.Id }, entity);
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IconFileDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddIconFileFromForm([FromForm] AddIconFileRequest request)
    {
        IconFileDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? BadRequest() : CreatedAtAction(nameof(GetIcon), new { id = entity.Id }, entity);
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(IEnumerable<IconFileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CacheIconsFromVideoStreamsRequest()
    {
        _ = await Mediator.Send(new CacheIconsFromVideoStreamsRequest()).ConfigureAwait(false);
        return Ok();
    }

    [HttpGet]
    [Route("[action]/{id}")]
    [ProducesResponseType(typeof(IconFileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IconFileDto>> GetIcon(int id)
    {
        var host = GetUrl();
        IconFileDto? data = await Mediator.Send(new GetIcon(id)).ConfigureAwait(false);
        return data != null ? (ActionResult<IconFileDto>)data : (ActionResult<IconFileDto>)NotFound();
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(IEnumerable<IconFileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<IconFileDto>>> GetIcons()
    {
        IEnumerable<IconFileDto> data = await Mediator.Send(new GetIcons()).ConfigureAwait(false);
        return data.ToList();
    }

    public string GetUrl()
    {
        var request = HttpContext.Request;
        var scheme = request.Scheme;
        var host = request.Host;
        var path = request.Path;
        var queryString = request.QueryString;

        var url = $"{scheme}://{host}";

        return url;
    }
}
