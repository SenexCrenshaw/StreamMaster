using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.M3UFiles;
using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.M3UFiles.Queries;
using StreamMaster.Application.Services;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.API.Controllers;

public class M3UFilesController(IBackgroundTaskQueue taskQueue) : ApiControllerBase, IM3UFileController
{


    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> CreateM3UFile(CreateM3UFileRequest request)
    {
        bool result = await Mediator.Send(request).ConfigureAwait(false);
        return result ? Ok() : BadRequest();
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> CreateM3UFileFromForm([FromForm] CreateM3UFileRequest request)
    {
        bool result = await Mediator.Send(request).ConfigureAwait(false);
        return result ? Ok() : BadRequest();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> ChangeM3UFileName(ChangeM3UFileNameRequest request)
    {
        bool result = await Mediator.Send(request).ConfigureAwait(false) != null;
        return result ? Ok() : BadRequest();
    }

    [HttpDelete]
    [Route("[action]")]
    public async Task<ActionResult> DeleteM3UFile(DeleteM3UFileRequest request)
    {
        int? data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<M3UFileDto>> GetM3UFile(int id)
    {
        M3UFileDto? data = await Mediator.Send(new GetM3UFileQuery(id)).ConfigureAwait(false);

        return data != null ? (ActionResult<M3UFileDto>)data : (ActionResult<M3UFileDto>)NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<M3UFileDto>>> GetPagedM3UFiles([FromQuery] M3UFileParameters Parameters)
    {
        PagedResponse<M3UFileDto> m3uFiles = await Mediator.Send(new GetPagedM3UFiles(Parameters)).ConfigureAwait(false);

        return Ok(m3uFiles);
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> ProcessM3UFile(ProcessM3UFileRequest request)
    {
        await taskQueue.ProcessM3UFile(request).ConfigureAwait(false);
        //M3UFile? data = await Mediator.Send(request).ConfigureAwait(false);
        return NoContent();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> RefreshM3UFile(RefreshM3UFileRequest request)
    {
        RefreshM3UFileRequest re = new(request.Id, true);
        M3UFile? data = await Mediator.Send(re).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> ScanDirectoryForM3UFiles()
    {
        _ = await Mediator.Send(new ScanDirectoryForM3UFilesRequest()).ConfigureAwait(false);
        return NoContent();
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult> UpdateM3UFile(UpdateM3UFileRequest request)
    {
        M3UFile? data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<string>>> GetM3UFileNames()
    {
        List<string> res = await Mediator.Send(new GetM3UFileNamesQuery()).ConfigureAwait(false);

        return Ok(res);
    }
}