using StreamMaster.Application.Settings;
using StreamMaster.Application.Settings.Commands;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : ISettingHub
{

    public async Task<SettingDto> GetSetting()
    {
        return await Sender.Send(new GetSettings()).ConfigureAwait(false);
    }

    public async Task<SDSystemStatus> GetSystemStatus()
    {
        return await Sender.Send(new GetSystemStatus()).ConfigureAwait(false);
    }

    public async Task<bool> LogIn(LogInRequest logInRequest)
    {

        return settings.AdminUserName == logInRequest.UserName && settings.AdminPassword == logInRequest.Password;
    }



    public async Task UpdateSetting(UpdateSettingRequest command)
    {
        _ = await Sender.Send(command).ConfigureAwait(false);
    }
}
