using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.EPGFiles;
using StreamMasterApplication.EPGFiles.Commands;
using StreamMasterApplication.EPGFiles.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterAPI.Controllers;

public class EPGFilesController : ApiControllerBase, IEPGFileController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EPGFilesDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddEPGFile(AddEPGFileRequest request)
    {
        EPGFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? Ok() : CreatedAtAction(nameof(GetEPGFile), new { id = entity.Id }, entity);
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EPGFilesDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddEPGFileFromForm([FromForm] AddEPGFileRequest request)
    {
        EPGFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? BadRequest() : CreatedAtAction(nameof(GetEPGFile), new { id = entity.Id }, entity);
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ChangeEPGFileName(ChangeEPGFileNameRequest request)
    {
        EPGFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    [HttpDelete]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteEPGFile(DeleteEPGFileRequest request)
    {
        int? data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(EPGFilesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EPGFilesDto>> GetEPGFile(int id)
    {
        EPGFilesDto? data = await Mediator.Send(new GetEPGFile(id)).ConfigureAwait(false);
        return data != null ? (ActionResult<EPGFilesDto>)data : (ActionResult<EPGFilesDto>)NotFound();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EPGFilesDto>))]
    public async Task<ActionResult<IEnumerable<EPGFilesDto>>> GetEPGFiles()
    {
        IEnumerable<EPGFilesDto> data = await Mediator.Send(new GetEPGFiles()).ConfigureAwait(false);
        return data.ToList();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ProcessEPGFile(ProcessEPGFileRequest request)
    {
        EPGFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RefreshEPGFile(RefreshEPGFileRequest request)
    {
        EPGFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ScanDirectoryForEPGFiles()
    {
        _ = await Mediator.Send(new ScanDirectoryForEPGFilesRequest()).ConfigureAwait(false);

        return NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateEPGFile(UpdateEPGFileRequest request)
    {
        EPGFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }
}
