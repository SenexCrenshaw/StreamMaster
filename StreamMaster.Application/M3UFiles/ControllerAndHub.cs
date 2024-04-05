using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.M3UFiles.Queries;

namespace StreamMaster.Application.M3UFiles
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
        public async Task<ActionResult<DefaultAPIResponse>> CreateM3UFile(CreateM3UFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> DeleteM3UFile(DeleteM3UFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> ProcessM3UFiles()
        {
            DefaultAPIResponse ret = await Sender.Send(new ProcessM3UFilesRequest()).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> RefreshM3UFile(RefreshM3UFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> UpdateM3UFile(UpdateM3UFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
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

        public async Task<DefaultAPIResponse> CreateM3UFile(CreateM3UFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> DeleteM3UFile(DeleteM3UFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> ProcessM3UFile(ProcessM3UFileRequest request)
        {
            await taskQueue.ProcessM3UFile(request).ConfigureAwait(false);
            return DefaultAPIResponse.Ok;
        }

        public async Task<DefaultAPIResponse> ProcessM3UFiles()
        {
            DefaultAPIResponse ret = await Sender.Send(new ProcessM3UFilesRequest()).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> RefreshM3UFile(RefreshM3UFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> UpdateM3UFile(UpdateM3UFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
