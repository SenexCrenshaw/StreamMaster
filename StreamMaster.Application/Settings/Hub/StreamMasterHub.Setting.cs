using StreamMaster.Application.Settings;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub
{
    public async Task<bool> LogIn(LogInRequest logInRequest)
    {
        return await Task.FromResult(settings.AdminUserName == logInRequest.UserName && settings.AdminPassword == logInRequest.Password);
    }


}
