using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.M3UFiles.Commands;

namespace StreamMaster.Application.M3UFiles
{
    public partial class M3UFilesController(ISender Sender) : ApiControllerBase, IM3UFilesController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse<M3UFileDto>>> GetPagedM3UFiles([FromQuery] M3UFileParameters Parameters)
        {
            APIResponse<M3UFileDto> ret = await Sender.Send(new GetPagedM3UFiles(Parameters)).ConfigureAwait(false);
            return ret;
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IM3UFilesHub
    {
        public async Task<DefaultAPIResponse> CreateM3UFile(CreateM3UFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse<M3UFileDto>> GetPagedM3UFiles(M3UFileParameters Parameters)
        {
            APIResponse<M3UFileDto> ret = await Sender.Send(new GetPagedM3UFiles(Parameters)).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> ProcessM3UFile(ProcessM3UFileRequest request)
        {
            await taskQueue.ProcessM3UFile(request).ConfigureAwait(false);
            return APIResponseFactory.Ok;
        }

    }
}
