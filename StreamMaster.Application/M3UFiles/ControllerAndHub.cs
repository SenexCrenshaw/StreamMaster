using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.M3UFiles.Queries;

namespace StreamMaster.Application.M3UFiles.Controllers
{
    public partial class M3UFilesController(ISender Sender, ILogger<M3UFilesController> _logger) : ApiControllerBase, IM3UFilesController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<PagedResponse<M3UFileDto>>> GetPagedM3UFiles([FromQuery] QueryStringParameters Parameters)
        {
            PagedResponse<M3UFileDto> ret = await Sender.Send(new GetPagedM3UFilesRequest(Parameters)).ConfigureAwait(false);
            return ret;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> CreateM3UFile(CreateM3UFileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> DeleteM3UFile(DeleteM3UFileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> ProcessM3UFiles()
        {
            APIResponse ret = await Sender.Send(new ProcessM3UFilesRequest()).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> RefreshM3UFile(RefreshM3UFileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> UpdateM3UFile(UpdateM3UFileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IM3UFilesHub
    {
        public async Task<PagedResponse<M3UFileDto>> GetPagedM3UFiles(QueryStringParameters Parameters)
        {
            PagedResponse<M3UFileDto> ret = await Sender.Send(new GetPagedM3UFilesRequest(Parameters)).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> CreateM3UFile(CreateM3UFileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> DeleteM3UFile(DeleteM3UFileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> ProcessM3UFile(ProcessM3UFileRequest request)
        {
            await taskQueue.ProcessM3UFile(request).ConfigureAwait(false);
            return APIResponse.Ok;
        }

        public async Task<APIResponse> ProcessM3UFiles()
        {
            APIResponse ret = await Sender.Send(new ProcessM3UFilesRequest()).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> RefreshM3UFile(RefreshM3UFileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> UpdateM3UFile(UpdateM3UFileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
