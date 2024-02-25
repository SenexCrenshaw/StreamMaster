using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Settings.Commands;

namespace StreamMaster.Application.Settings;

public interface ISettingController
{
    Task<ActionResult<SettingDto>> GetSetting();

    Task<ActionResult<SDSystemStatus>> GetSystemStatus();

    ActionResult<bool> LogIn(LogInRequest logInRequest);

    Task<ActionResult<UpdateSettingResponse>> AddFFMPEGProfile(AddFFMPEGProfileRequest request);
    Task<ActionResult<UpdateSettingResponse>> RemoveFFMPEGProfile(RemoveFFMPEGProfileRequest request);
    Task<ActionResult<UpdateSettingResponse>> UpdateFFMPEGProfile(UpdateFFMPEGProfileRequest request);
    Task<IActionResult> UpdateSetting(UpdateSettingRequest command);
}


public interface ISettingHub
{
    Task<UpdateSettingResponse> UpdateFFMPEGProfile(UpdateFFMPEGProfileRequest request);
    Task<UpdateSettingResponse> AddFFMPEGProfile(AddFFMPEGProfileRequest request);
    Task<UpdateSettingResponse> RemoveFFMPEGProfile(RemoveFFMPEGProfileRequest request);
    Task<SettingDto> GetSetting();

    Task<SDSystemStatus> GetSystemStatus();

    Task<bool> LogIn(LogInRequest logInRequest);

    Task UpdateSetting(UpdateSettingRequest command);
}

public interface ISettingTasks
{
    ValueTask SetIsSystemReady(bool isSystemReady, CancellationToken cancellationToken = default);
}
