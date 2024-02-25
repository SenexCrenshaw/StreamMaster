using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.General.Queries;
using StreamMaster.Application.Settings;
using StreamMaster.Application.Settings.Commands;
using StreamMaster.Application.Settings.Queries;

namespace StreamMaster.API.Controllers;

public class SettingsController() : ApiControllerBase, ISettingController
{
    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult<UpdateSettingResponse>> AddFFMPEGProfile(AddFFMPEGProfileRequest request)
    {
        return await Mediator.Send(request).ConfigureAwait(false);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<bool>> GetIsSystemReady()
    {
        return await Mediator.Send(new GetIsSystemReadyRequest()).ConfigureAwait(false);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<SettingDto>> GetSetting()
    {
        return await Mediator.Send(new GetSettings()).ConfigureAwait(false);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("[action]")]
    public async Task<ActionResult<SDSystemStatus>> GetSystemStatus()
    {
        return await Mediator.Send(new GetSystemStatus()).ConfigureAwait(false);
    }

    [HttpGet]
    [Route("[action]")]
    public ActionResult<bool> LogIn(LogInRequest logInRequest)
    {
        return Settings.AdminUserName == logInRequest.UserName && Settings.AdminPassword == logInRequest.Password;
    }

    [HttpDelete]
    [Route("[action]")]
    public async Task<ActionResult<UpdateSettingResponse>> RemoveFFMPEGProfile(RemoveFFMPEGProfileRequest request)
    {
        return await Mediator.Send(request).ConfigureAwait(false);
    }

    [HttpPatch]
    [Route("[action]")]
    public async Task<IActionResult> UpdateSetting(UpdateSettingRequest command)
    {
        UpdateSettingResponse updateSettingResponse = await Mediator.Send(command).ConfigureAwait(false);

        return updateSettingResponse.NeedsLogOut ? Redirect("/logout") : updateSettingResponse == null ? NotFound() : NoContent();
    }
}