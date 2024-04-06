using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.M3UFiles.Commands;

namespace StreamMaster.Application.M3UFiles.Controllers;

public partial class M3UFilesController : ApiControllerBase
{

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult<APIResponse>> CreateM3UFileFromForm([FromForm] CreateM3UFileRequest request)
    {
        APIResponse entity = await Sender.Send(request).ConfigureAwait(false);
        return entity == null ? APIResponse.Error : APIResponse.Success;
    }
}