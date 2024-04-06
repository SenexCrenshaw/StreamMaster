namespace StreamMaster.Application.Profiles.Commands;

public record UpdateFFMPEGProfileRequest(string Name, string? NewName, string? Parameters, int? TimeOut, bool? IsM3U8) : IRequest<UpdateSettingResponse> { }

public class UpdateFFMPEGProfileRequestHandler(
    ILogger<UpdateFFMPEGProfileRequest> Logger,
    IOptionsMonitor<FFMPEGProfiles> intprofilesettings,
    IMapper Mapper,
    ISender Sender,
    IRepositoryWrapper repositoryWrapper
    )
: IRequestHandler<UpdateFFMPEGProfileRequest, UpdateSettingResponse>
{

    private readonly FFMPEGProfiles profilesettings = intprofilesettings.CurrentValue;

    public async Task<UpdateSettingResponse> Handle(UpdateFFMPEGProfileRequest request, CancellationToken cancellationToken)
    {
        if (!profilesettings.Profiles.ContainsKey(request.Name))
        {
            DataResponse<SettingDto> ret1 = await Sender.Send(new GetSettingsRequest(), cancellationToken);
            return new UpdateSettingResponse { Settings = ret1.Data, NeedsLogOut = false };
        }

        if (profilesettings.Profiles.TryGetValue(request.Name, out FFMPEGProfile? existingProfile))
        {

            if (request.Parameters != null)
            {
                existingProfile.Parameters = request.Parameters;
            }
            if (request.TimeOut != null)
            {
                existingProfile.Timeout = request.TimeOut.Value;
            }
            if (request.IsM3U8 != null)
            {
                existingProfile.IsM3U8 = request.IsM3U8.Value;
            }
            if (request.NewName != null)
            {
                profilesettings.Profiles.Remove(request.Name);
                profilesettings.Profiles.Add(request.NewName, existingProfile);
                repositoryWrapper.StreamGroup.GetQuery(x => x.FFMPEGProfileId == request.Name).ToList().ForEach(x => x.FFMPEGProfileId = request.NewName);
                await repositoryWrapper.SaveAsync();
            }
            Logger.LogInformation("UpdateFFMPEGProfileRequest");

            SettingsHelper.UpdateSetting(profilesettings);

        }
        DataResponse<SettingDto> ret = await Sender.Send(new GetSettingsRequest(), cancellationToken);
        return new UpdateSettingResponse { Settings = ret.Data, NeedsLogOut = false };
    }

}