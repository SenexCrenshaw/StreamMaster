using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Settings.Commands;

namespace StreamMaster.Application.Settings;

public interface ISettingController
{
    ActionResult<bool> LogIn(LogInRequest logInRequest);

    Task<IActionResult> UpdateSetting(UpdateSettingRequest command);
}

public interface ISettingHub
{
    Task<bool> LogIn(LogInRequest logInRequest);

    Task UpdateSetting(UpdateSettingRequest command);
}

public interface ISettingTasks
{
    ValueTask SetIsSystemReady(bool isSystemReady, CancellationToken cancellationToken = default);
    ValueTask SetTestTask(int DelayInSeconds, CancellationToken cancellationToken = default);
}
