using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.EPGFiles.Commands;

namespace StreamMaster.Application.EPGFiles.Controllers;

public partial class EPGFilesController : ApiControllerBase
{
    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult<APIResponse>> CreateEPGFileFromForm([FromForm] CreateEPGFileRequest request)
    {
        APIResponse entity = await Sender.Send(request).ConfigureAwait(false);
        return entity == null ? APIResponse.Error : APIResponse.Success;
    }
}