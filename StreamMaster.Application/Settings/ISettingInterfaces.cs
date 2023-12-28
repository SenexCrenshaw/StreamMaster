using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Common.Models;
using StreamMaster.Application.Settings.Commands;

namespace StreamMaster.Application.Settings;

public interface ISettingController
{
    Task<ActionResult<List<TaskQueueStatus>>> GetQueueStatus();

    Task<ActionResult<SettingDto>> GetSetting();

    Task<ActionResult<SDSystemStatus>> GetSystemStatus();

    Task<ActionResult<bool>> LogIn(LogInRequest logInRequest);

    Task<IActionResult> UpdateSetting(UpdateSettingRequest command);
}

public interface ISettingDB
{
}

public interface ISettingHub
{
    Task<List<TaskQueueStatus>> GetQueueStatus();

    Task<SettingDto> GetSetting();

    Task<SDSystemStatus> GetSystemStatus();

    Task<bool> LogIn(LogInRequest logInRequest);

    Task UpdateSetting(UpdateSettingRequest command);
}

public interface ISettingScoped
{
}

public interface ISettingTasks
{
    ValueTask SetIsSystemReady(bool isSystemReady, CancellationToken cancellationToken = default);
}
