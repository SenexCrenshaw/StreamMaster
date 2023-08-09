using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.EPGFiles;
using StreamMasterApplication.EPGFiles.Commands;
using StreamMasterApplication.EPGFiles.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterAPI.Controllers;

public class EPGFilesController : ApiControllerBase, IEPGFileController
{
    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> CreateEPGFile(CreateEPGFileRequest request)
    {
        EPGFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? Ok() : CreatedAtAction(nameof(GetEPGFile), new { id = entity.Id }, entity);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> CreateEPGFileFromForm([FromForm] CreateEPGFileRequest request)
    {
        EPGFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
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
    [Route("{id}")]
    public async Task<ActionResult<EPGFilesDto>> GetEPGFile(int id)
    {
        EPGFilesDto? data = await Mediator.Send(new GetEPGFile(id)).ConfigureAwait(false);
        return data != null ? (ActionResult<EPGFilesDto>)data : (ActionResult<EPGFilesDto>)NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IPagedList<EPGFilesDto>>> GetEPGFiles([FromQuery] EPGFileParameters parameters)
    {
        var data = await Mediator.Send(new GetEPGFiles(parameters)).ConfigureAwait(false);
        return data;
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> ProcessEPGFile(ProcessEPGFileRequest request)
    {
        EPGFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> RefreshEPGFile(RefreshEPGFileRequest request)
    {
        EPGFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> ScanDirectoryForEPGFiles()
    {
        _ = await Mediator.Send(new ScanDirectoryForEPGFilesRequest()).ConfigureAwait(false);

        return NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> UpdateEPGFile(UpdateEPGFileRequest request)
    {
        EPGFilesDto? entity = await Mediator.Send(request).ConfigureAwait(false);
        return entity == null ? NotFound() : NoContent();
    }
}