using StreamMaster.Application.Profiles.Commands;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IProfilesHub
{
    public async Task<FFMPEGProfileDtos> GetFFMPEGProfiles()
    {
        return await Sender.Send(new GetFFMPEGProfiles()).ConfigureAwait(false);
    }

    public async Task<UpdateSettingResponse> AddFFMPEGProfile(AddFFMPEGProfileRequest request)
    {
        return await Sender.Send(request).ConfigureAwait(false);
    }


    public async Task<UpdateSettingResponse> RemoveFFMPEGProfile(RemoveFFMPEGProfileRequest request)
    {
        return await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task<UpdateSettingResponse> UpdateFFMPEGProfile(UpdateFFMPEGProfileRequest request)
    {
        return await Sender.Send(request).ConfigureAwait(false);
    }
}
