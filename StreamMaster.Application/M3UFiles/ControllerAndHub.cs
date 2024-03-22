using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.M3UFiles.Commands;

namespace StreamMaster.Application.M3UFiles
{
    public partial class M3UFilesController(ISender Sender) : ApiControllerBase, IM3UFilesController
    {        

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IM3UFilesHub
    {
        public async Task<DefaultAPIResponse?> ProcessM3UFile(ProcessM3UFileRequest request)
        {
            await taskQueue.ProcessM3UFile(request).ConfigureAwait(false);
            return APIResponseFactory.Ok;
        }

    }
}
