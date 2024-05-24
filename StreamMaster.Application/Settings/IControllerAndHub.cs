using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Settings.Commands;
using StreamMaster.Application.Settings.Queries;

namespace StreamMaster.Application.Settings
{
    public interface ISettingsController
    {        
        Task<ActionResult<bool>> GetIsSystemReady();
        Task<ActionResult<SettingDto>> GetSettings();
        Task<ActionResult<SDSystemStatus>> GetSystemStatus();
        Task<ActionResult<UpdateSettingResponse?>> UpdateSetting(UpdateSettingRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISettingsHub
    {
        Task<bool> GetIsSystemReady();
        Task<SettingDto> GetSettings();
        Task<SDSystemStatus> GetSystemStatus();
        Task<UpdateSettingResponse?> UpdateSetting(UpdateSettingRequest request);
    }
}
