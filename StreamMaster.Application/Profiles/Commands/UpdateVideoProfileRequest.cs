namespace StreamMaster.Application.Profiles.Commands;


[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateVideoProfileRequest(string Name, string? NewName, string? Parameters, int? TimeOut, bool? IsM3U8)
    : IRequest<APIResponse>
{ }

public class UpdateVideoProfileRequestHandler(
    ILogger<UpdateVideoProfileRequest> Logger,
    IOptionsMonitor<VideoOutputProfiles> intprofilesettings,
    IMapper Mapper,
    ISender Sender,
    IRepositoryWrapper repositoryWrapper
    )
: IRequestHandler<UpdateVideoProfileRequest, APIResponse>
{

    private readonly VideoOutputProfiles profilesettings = intprofilesettings.CurrentValue;

    public async Task<APIResponse> Handle(UpdateVideoProfileRequest request, CancellationToken cancellationToken)
    {
        if (!profilesettings.VideoProfiles.ContainsKey(request.Name))
        {
            DataResponse<SettingDto> ret1 = await Sender.Send(new GetSettingsRequest(), cancellationToken);
            //return APIResponse.Success(new UpdateSettingResponse { Settings = ret1.Data, NeedsLogOut = false });
            return APIResponse.Ok;
        }

        if (profilesettings.VideoProfiles.TryGetValue(request.Name, out VideoOutputProfile? existingProfile))
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
                profilesettings.VideoProfiles.Remove(request.Name);
                profilesettings.VideoProfiles.Add(request.NewName, existingProfile);
                //repositoryWrapper.StreamGroup.GetQuery(x => x.VideoProfileId == request.Name).ToList().ForEach(x => x.VideoProfileId = request.NewName);
                await repositoryWrapper.SaveAsync();
            }
            Logger.LogInformation("UpdateVideoProfileRequest");

            SettingsHelper.UpdateSetting(profilesettings);

        }
        DataResponse<SettingDto> ret = await Sender.Send(new GetSettingsRequest(), cancellationToken);
        //return APIResponse.Success(new UpdateSettingResponse { Settings = ret.Data, NeedsLogOut = false });
        return APIResponse.Ok;
    }

}