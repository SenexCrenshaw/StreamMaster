using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Icons;
using StreamMasterApplication.Icons.Commands;
using StreamMasterApplication.Icons.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterAPI.Controllers;

public class IconsController : ApiControllerBase, IIconController
{
    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpGet]
    [Route("[action]/{id}")]
    public async Task<ActionResult<IconFileDto>> GetIcon(int id)
    {
        IconFileDto? data = await Mediator.Send(new GetIcon(id)).ConfigureAwait(false);
        return data != null ? (ActionResult<IconFileDto>)data : (ActionResult<IconFileDto>)NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IconFileDto>>> GetIcons([FromQuery] IconFileParameters iconFileParameters)
    {
        IPagedList<IconFileDto> result = await Mediator.Send(new GetIcons(iconFileParameters)).ConfigureAwait(false);

        //var result = await PagedList<IconFileDto>.ToPagedList(data.AsQueryable(), iconFileParameters.PageNumber, iconFileParameters.PageSize);

        return Ok(result);
    }
    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<IconSimpleDto>>> GetIconsSimpleQuery()
    {
        IEnumerable<IconSimpleDto> result = await Mediator.Send(new GetIconsSimpleQuery()).ConfigureAwait(false);
        return Ok(result);
    }
}