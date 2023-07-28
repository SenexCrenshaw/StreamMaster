using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Settings.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Settings;

public interface ISettingController
{
    Task<ActionResult<List<TaskQueueStatusDto>>> GetQueueStatus();

    Task<ActionResult<SettingDto>> GetSetting();

    Task<ActionResult<SystemStatus>> GetSystemStatus();

    ActionResult<bool> LogIn(LogInRequest logInRequest);

    Task<IActionResult> UpdateSetting(UpdateSettingRequest command);
}

public interface ISettingDB
{
}

public interface ISettingHub
{
    Task<List<TaskQueueStatusDto>> GetQueueStatus();

    Task<SettingDto> GetSetting();

    Task<SystemStatus> GetSystemStatus();

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
