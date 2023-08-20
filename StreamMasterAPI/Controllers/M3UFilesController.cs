using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.M3UFiles;
using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.M3UFiles.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;

namespace StreamMasterAPI.Controllers;

public class M3UFilesController : ApiControllerBase, IM3UFileController
{

    public M3UFilesController()
    {

    }

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

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> ChangeM3UFileName(ChangeM3UFileNameRequest request)
    {
        bool result = await Mediator.Send(request).ConfigureAwait(false);
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
        M3UFileDto? data = await Mediator.Send(new GetM3UFileByIdQuery(id)).ConfigureAwait(false);

        return data != null ? (ActionResult<M3UFileDto>)data : (ActionResult<M3UFileDto>)NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<M3UFileDto>>> GetM3UFiles([FromQuery] M3UFileParameters Parameters)
    {
        PagedResponse<M3UFileDto> m3uFiles = await Mediator.Send(new GetM3UFilesQuery(Parameters)).ConfigureAwait(false);

        return Ok(m3uFiles);
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> ProcessM3UFile(ProcessM3UFileRequest request)
    {
        M3UFile? data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> RefreshM3UFile(RefreshM3UFileRequest request)
    {
        M3UFile? data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> ScanDirectoryForM3UFiles()
    {
        _ = await Mediator.Send(new ScanDirectoryForM3UFilesRequest()).ConfigureAwait(false);
        return NoContent();
    }

    [HttpPut]
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