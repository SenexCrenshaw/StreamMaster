using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.M3UFiles.Commands;

namespace StreamMaster.Application.M3UFiles;

public partial class M3UFilesController
{

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult<DefaultAPIResponse>> CreateM3UFileFromForm([FromForm] CreateM3UFileRequest request)
    {
        DefaultAPIResponse entity = await Sender.Send(request).ConfigureAwait(false);
        return entity == null ? DefaultAPIResponse.Error : DefaultAPIResponse.Success;
    }
}