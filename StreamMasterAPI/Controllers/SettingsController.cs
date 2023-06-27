using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.General.Queries;
using StreamMasterApplication.Settings;
using StreamMasterApplication.Settings.Commands;
using StreamMasterApplication.Settings.Queries;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using static StreamMasterApplication.Settings.Commands.UpdateSettingHandler;

namespace StreamMasterAPI.Controllers;

public class SettingsController : ApiControllerBase, ISettingController
{
    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> GetIsSystemReady()
    {
        return await Mediator.Send(new GetIsSystemReadyRequest()).ConfigureAwait(false);
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(List<TaskQueueStatusDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TaskQueueStatusDto>>> GetQueueStatus()
    {
        return await Mediator.Send(new GetQueueStatus()).ConfigureAwait(false);
    }

    [HttpGet]
    [ProducesResponseType(typeof(SettingDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SettingDto>> GetSetting()
    {
        return await Mediator.Send(new GetSettings()).ConfigureAwait(false);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("[action]")]
    [ProducesResponseType(typeof(SystemStatus), StatusCodes.Status200OK)]
    public async Task<ActionResult<SystemStatus>> GetSystemStatus()
    {
        return await Mediator.Send(new GetSystemStatus()).ConfigureAwait(false);
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public ActionResult<bool> LogIn(LogInRequest logInRequest)
    {
        var setting = FileUtil.GetSetting();

        return setting.AdminUserName == logInRequest.UserName && setting.AdminPassword == logInRequest.Password;
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(typeof(UpdateSettingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateSetting(UpdateSettingRequest command)
    {
        UpdateSettingResponse updateSettingResponse = await Mediator.Send(command).ConfigureAwait(false);

        if (updateSettingResponse.NeedsLogOut)
        {
            return Redirect("/logout");
        }

        return updateSettingResponse == null ? NotFound() : NoContent();
    }
}
