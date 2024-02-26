using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Profiles.Commands;

namespace StreamMaster.API.Controllers;

public class ProfilesController : ApiControllerBase, IProfilesController
{

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<FFMPEGProfileDtos>> GetFFMPEGProfiles()
    {
        return await Mediator.Send(new GetFFMPEGProfiles()).ConfigureAwait(false);
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult<UpdateSettingResponse>> AddFFMPEGProfile(AddFFMPEGProfileRequest request)
    {
        return await Mediator.Send(request).ConfigureAwait(false);
    }


    [HttpDelete]
    [Route("[action]")]
    public async Task<ActionResult<UpdateSettingResponse>> RemoveFFMPEGProfile(RemoveFFMPEGProfileRequest request)
    {
        return await Mediator.Send(request).ConfigureAwait(false);
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<ActionResult<UpdateSettingResponse>> UpdateFFMPEGProfile(UpdateFFMPEGProfileRequest request)
    {
        return await Mediator.Send(request).ConfigureAwait(false);
    }
}