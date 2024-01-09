﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.General.Queries;
using StreamMaster.Application.Settings;
using StreamMaster.Application.Settings.Commands;
using StreamMaster.Application.Settings.Queries;
using StreamMaster.Domain.Dto;

namespace StreamMaster.API.Controllers;

public class SettingsController : ApiControllerBase, ISettingController
{

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

    [HttpPatch]
    [Route("[action]")]
    public async Task<IActionResult> UpdateSetting(UpdateSettingRequest command)
    {
        UpdateSettingRequestHandler.UpdateSettingResponse updateSettingResponse = await Mediator.Send(command).ConfigureAwait(false);

        return updateSettingResponse.NeedsLogOut ? Redirect("/logout") : updateSettingResponse == null ? NotFound() : NoContent();
    }
}