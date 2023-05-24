using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.M3UFiles;
using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.M3UFiles.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterAPI.Controllers;

public class M3UFilesController : ApiControllerBase, IM3UFileController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(M3UFilesDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddM3UFile(AddM3UFileRequest request)
    {
        M3UFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? Ok() : CreatedAtAction(nameof(GetM3UFile), new { id = entity.Id }, entity);
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(M3UFilesDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddM3UFileFromForm([FromForm] AddM3UFileRequest request)
    {
        M3UFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? BadRequest() : CreatedAtAction(nameof(GetM3UFile), new { id = entity.Id }, entity);
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ChangeM3UFileName(ChangeM3UFileNameRequest request)
    {
        M3UFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    [HttpDelete]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteM3UFile(DeleteM3UFileRequest request)
    {
        int? data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(M3UFilesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<M3UFilesDto>> GetM3UFile(int id)
    {
        M3UFilesDto? data = await Mediator.Send(new GetM3UFile(id)).ConfigureAwait(false);

        return data != null ? (ActionResult<M3UFilesDto>)data : (ActionResult<M3UFilesDto>)NotFound();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<M3UFilesDto>))]
    public async Task<ActionResult<IEnumerable<M3UFilesDto>>> GetM3UFiles()
    {
        IEnumerable<M3UFilesDto> data = await Mediator.Send(new GetM3UFiles()).ConfigureAwait(false);
        return data.ToList();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ProcessM3UFile(ProcessM3UFileRequest request)
    {
        M3UFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RefreshM3UFile(RefreshM3UFileRequest request)
    {
        M3UFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ScanDirectoryForM3UFiles()
    {
        _ = await Mediator.Send(new ScanDirectoryForM3UFilesRequest()).ConfigureAwait(false);

        return NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateM3UFile(UpdateM3UFileRequest request)
    {
        M3UFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }
}
