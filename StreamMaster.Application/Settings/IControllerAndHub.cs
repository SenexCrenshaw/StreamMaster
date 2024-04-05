using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Settings.Queries;

namespace StreamMaster.Application.Settings
{
    public interface ISettingsController
    {        
        Task<ActionResult<bool>> GetIsSystemReady();
        Task<ActionResult<SettingDto>> GetSettings();
        Task<ActionResult<SDSystemStatus>> GetSystemStatus();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISettingsHub
    {
        Task<bool> GetIsSystemReady();
        Task<SettingDto> GetSettings();
        Task<SDSystemStatus> GetSystemStatus();
    }
}
