using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Settings;
using StreamMasterApplication.Settings.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : ISettingHub
{
    public async Task<List<TaskQueueStatusDto>> GetQueueStatus()
    {
        return await _mediator.Send(new GetQueueStatus()).ConfigureAwait(false);
    }

    public async Task<SettingDto> GetSetting()
    {
        return await _mediator.Send(new GetSettings()).ConfigureAwait(false);
    }

    public async Task<SystemStatus> GetSystemStatus()
    {
        return await _mediator.Send(new GetSystemStatus()).ConfigureAwait(false);
    }

    public Task<bool> LogIn(LogInRequest logInRequest)
    {
        var setting = FileUtil.GetSetting();

        return Task.FromResult(setting.AdminUserName == logInRequest.UserName && setting.AdminPassword == logInRequest.Password);
    }

    public async Task<SettingDto?> UpdateSetting(UpdateSettingRequest command)
    {
        return await _mediator.Send(command).ConfigureAwait(false);
    }
}
