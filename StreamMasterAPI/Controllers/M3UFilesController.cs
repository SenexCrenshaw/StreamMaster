using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using StreamMasterAPI.Extensions;

using StreamMasterApplication.M3UFiles;
using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.M3UFiles.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;

namespace StreamMasterAPI.Controllers;

public class M3UFilesController : ApiControllerBase, IM3UFileController
{
    private readonly IMapper _mapper;

    public M3UFilesController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> CreateM3UFile(CreateM3UFileRequest request)
    {
        var result = await Mediator.Send(request).ConfigureAwait(false);
        return result ? Ok() : BadRequest();
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> CreateM3UFileFromForm([FromForm] CreateM3UFileRequest request)
    {
        var result = await Mediator.Send(request).ConfigureAwait(false);
        return result ? Ok() : BadRequest();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> ChangeM3UFileName(ChangeM3UFileNameRequest request)
    {
        var result = await Mediator.Send(request).ConfigureAwait(false);
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
    public async Task<ActionResult<PagedList<M3UFileDto>>> GetM3UFiles([FromQuery] M3UFileParameters Parameters)
    {
        var m3uFiles = await Mediator.Send(new GetM3UFilesQuery(Parameters)).ConfigureAwait(false);
        PagedList<M3UFileDto> result = APIExtensions.GetPagedResult<M3UFile, M3UFileDto>(m3uFiles, Response, _mapper);

        return Ok(result);
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> ProcessM3UFile(ProcessM3UFileRequest request)
    {
        var data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> RefreshM3UFile(RefreshM3UFileRequest request)
    {
        var data = await Mediator.Send(request).ConfigureAwait(false);
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
        var data = await Mediator.Send(request).ConfigureAwait(false);
        return data == null ? NotFound() : NoContent();
    }
}