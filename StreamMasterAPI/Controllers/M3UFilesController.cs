using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.M3UFiles;
using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.M3UFiles.Queries;

using StreamMasterDomain.Dto;

using StreamMasterInfrastructure.Pagination;

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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<M3UFileDto>))]
    public async Task<ActionResult<IEnumerable<M3UFileDto>>> GetM3UFiles(M3UFileParameters Parameters)
    {
        var m3uFiles = await Mediator.Send(new GetM3UFilesQuery(Parameters)).ConfigureAwait(false);
        Response.Headers.Add("X-Pagination", m3uFiles.GetMetadata());
        var m3uFilesResult = _mapper.Map<IEnumerable<M3UFileDto>>(m3uFiles);
        return Ok(m3uFilesResult);
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
