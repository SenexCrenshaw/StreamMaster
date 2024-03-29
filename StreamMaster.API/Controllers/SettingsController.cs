using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Settings;
using StreamMaster.Application.Settings.CommandsOld;

namespace StreamMaster.API.Controllers;

public class SettingsController() : ApiControllerBase, ISettingController
{

    [HttpGet]
    [Route("[action]")]
    public ActionResult<bool> LogIn(LogInRequest logInRequest)
    {
        return Settings.AdminUserName == logInRequest.UserName && Settings.AdminPassword == logInRequest.Password;
    }


    [HttpPatch]
    [Route("[action]")]
    public async Task<IActionResult> UpdateSetting(UpdateSettingRequest command)
    {
        UpdateSettingResponse updateSettingResponse = await Mediator.Send(command).ConfigureAwait(false);

        return updateSettingResponse.NeedsLogOut ? Redirect("/logout") : updateSettingResponse == null ? NotFound() : NoContent();
    }
}