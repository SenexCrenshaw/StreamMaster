namespace StreamMaster.Application.Profiles.Commands;


[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateFileProfileRequest(FileOutputProfileDto FileOutputProfileDto, string? NewName)
    : IRequest<APIResponse>
{ }

public class UpdateFileProfileRequestHandler(
    ILogger<UpdateFileProfileRequest> Logger,
    IOptionsMonitor<FileOutputProfiles> intprofilesettings,
    IMapper Mapper,
    ISender Sender,
    IRepositoryWrapper repositoryWrapper
    )
: IRequestHandler<UpdateFileProfileRequest, APIResponse>
{

    private readonly FileOutputProfiles profilesettings = intprofilesettings.CurrentValue;

    public async Task<APIResponse> Handle(UpdateFileProfileRequest request, CancellationToken cancellationToken)
    {
        if (!profilesettings.FileProfiles.ContainsKey(request.FileOutputProfileDto.Name))
        {
            DataResponse<SettingDto> ret1 = await Sender.Send(new GetSettingsRequest(), cancellationToken);
            //return APIResponse.Success(new UpdateSettingResponse { Settings = ret1.Data, NeedsLogOut = false });
            return APIResponse.Ok;
        }

        if (profilesettings.FileProfiles.TryGetValue(request.FileOutputProfileDto.Name, out FileOutputProfile? existingProfile))
        {

            if (request.FileOutputProfileDto.EPGOutputProfile != null)
            {
                existingProfile.EPGOutputProfile = request.FileOutputProfileDto.EPGOutputProfile;
            }
            if (request.FileOutputProfileDto.M3UOutputProfile != null)
            {
                existingProfile.M3UOutputProfile = request.FileOutputProfileDto.M3UOutputProfile;
            }

            if (!string.IsNullOrEmpty(request.NewName) && request.FileOutputProfileDto.Name != request.NewName)
            {
                profilesettings.FileProfiles.Remove(request.FileOutputProfileDto.Name);
                profilesettings.FileProfiles.Add(request.NewName, existingProfile);
                //repositoryWrapper.StreamGroup.GetQuery(x => x.FileProfileId == request.Name).ToList().ForEach(x => x.FileProfileId = request.NewName);
                await repositoryWrapper.SaveAsync();
            }
            Logger.LogInformation("UpdateFileProfileRequest");

            SettingsHelper.UpdateSetting(profilesettings);

        }
        DataResponse<SettingDto> ret = await Sender.Send(new GetSettingsRequest(), cancellationToken);
        //return APIResponse.Success(new UpdateSettingResponse { Settings = ret.Data, NeedsLogOut = false });
        return APIResponse.Ok;
    }

}