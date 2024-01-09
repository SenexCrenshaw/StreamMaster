using Microsoft.AspNetCore.Mvc;

using StreamMaster.API.Controllers;
using StreamMaster.Application.EPGFiles;
using StreamMaster.Application.EPGFiles.Commands;
using StreamMaster.Application.EPGFiles.Queries;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Pagination;

namespace StreamMasterAPI.Controllers;

public class EPGFilesController : ApiControllerBase, IEPGFileController
{
    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> CreateEPGFile(CreateEPGFileRequest request)
    {
        EPGFileDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? Ok() : CreatedAtAction(nameof(GetEPGFile), new { id = entity.Id }, entity);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> CreateEPGFileFromForm([FromForm] CreateEPGFileRequest request)
    {
        EPGFileDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? BadRequest() : CreatedAtAction(nameof(GetEPGFile), new { id = entity.Id }, entity);
    }

    [HttpDelete]
    [Route("[action]")]
    public async Task<ActionResult> DeleteEPGFile(DeleteEPGFileRequest request)
    {
        int? data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<EPGColorDto>>> GetEPGColors()
    {
        List<EPGColorDto> res = await Mediator.Send(new GetEPGColors()).ConfigureAwait(false);
        return Ok(res);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<EPGFileDto>> GetEPGFile(int id)
    {
        EPGFileDto? data = await Mediator.Send(new GetEPGFile(id)).ConfigureAwait(false);
        return data != null ? (ActionResult<EPGFileDto>)data : (ActionResult<EPGFileDto>)NotFound();
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<EPGFilePreviewDto>>> GetEPGFilePreviewById(int id)
    {
        return Ok(await Mediator.Send(new GetEPGFilePreviewById(id)).ConfigureAwait(false));
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<int>> GetEPGNextEPGNumber()
    {
        return Ok(await Mediator.Send(new GetEPGNextEPGNumber()).ConfigureAwait(false));
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<EPGFileDto>>> GetPagedEPGFiles([FromQuery] EPGFileParameters parameters)
    {
        PagedResponse<EPGFileDto> data = await Mediator.Send(new GetPagedEPGFiles(parameters)).ConfigureAwait(false);
        return data;
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> ProcessEPGFile(ProcessEPGFileRequest request)
    {
        EPGFileDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> RefreshEPGFile(RefreshEPGFileRequest request)
    {
        EPGFileDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> ScanDirectoryForEPGFiles()
    {
        _ = await Mediator.Send(new ScanDirectoryForEPGFilesRequest()).ConfigureAwait(false);

        return NoContent();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> UpdateEPGFile(UpdateEPGFileRequest request)
    {
        EPGFileDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }
}