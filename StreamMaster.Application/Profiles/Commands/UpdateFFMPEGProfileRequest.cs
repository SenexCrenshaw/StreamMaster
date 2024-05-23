namespace StreamMaster.Application.Profiles.Commands;


[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateFFMPEGProfileRequest(string Name, string? NewName, string? Parameters, int? TimeOut, bool? IsM3U8) : IRequest<APIResponse> { }

public class UpdateFFMPEGProfileRequestHandler(
    ILogger<UpdateFFMPEGProfileRequest> Logger,
    IOptionsMonitor<FFMPEGProfiles> intprofilesettings,
    IMapper Mapper,
    ISender Sender,
    IRepositoryWrapper repositoryWrapper
    )
: IRequestHandler<UpdateFFMPEGProfileRequest, APIResponse>
{

    private readonly FFMPEGProfiles profilesettings = intprofilesettings.CurrentValue;

    public async Task<APIResponse> Handle(UpdateFFMPEGProfileRequest request, CancellationToken cancellationToken)
    {
        if (!profilesettings.Profiles.ContainsKey(request.Name))
        {
            DataResponse<SettingDto> ret1 = await Sender.Send(new GetSettingsRequest(), cancellationToken);
            //return APIResponse.Success(new UpdateSettingResponse { Settings = ret1.Data, NeedsLogOut = false });
            return APIResponse.Ok;
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
        //return APIResponse.Success(new UpdateSettingResponse { Settings = ret.Data, NeedsLogOut = false });
        return APIResponse.Ok;
    }

}