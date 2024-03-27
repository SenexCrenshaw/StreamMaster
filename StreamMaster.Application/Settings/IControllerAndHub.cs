using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Settings.Commands;

namespace StreamMaster.Application.Settings
{
    public interface ISettingsController
    {        
    Task<ActionResult<SettingDto>> GetSettings();
    Task<ActionResult<SDSystemStatus>> GetSystemStatus();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISettingsHub
    {
        Task<SettingDto> GetSettings();
        Task<SDSystemStatus> GetSystemStatus();
    }
}