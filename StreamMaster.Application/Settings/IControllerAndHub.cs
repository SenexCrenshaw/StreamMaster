using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Settings.Commands;
using StreamMaster.Application.Settings.Queries;

namespace StreamMaster.Application.Settings
{
    public interface ISettingsController
    {        
        Task<ActionResult<SettingDto>> GetSettings();
        Task<ActionResult<UpdateSettingResponse?>> UpdateSetting(UpdateSettingRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISettingsHub
    {
        Task<SettingDto> GetSettings();
        Task<UpdateSettingResponse?> UpdateSetting(UpdateSettingRequest request);
    }
}
