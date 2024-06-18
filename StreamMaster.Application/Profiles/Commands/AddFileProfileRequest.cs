
namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddFileProfileRequest(FileOutputProfileDto FileOutputProfileDto) : IRequest<APIResponse> { }

public class AddFileVideoProfileRequestHandler(ILogger<AddFileProfileRequest> Logger,
    IMessageService messageService,
    IOptionsMonitor<FileOutputProfiles> intprofilesettings, IDataRefreshService dataRefreshService)
: IRequestHandler<AddFileProfileRequest, APIResponse>
{

    private readonly FileOutputProfiles profileSettings = intprofilesettings.CurrentValue;

    public async Task<APIResponse> Handle(AddFileProfileRequest request, CancellationToken cancellationToken)
    {

        if (profileSettings.FileProfiles.TryGetValue(request.FileOutputProfileDto.Name, out FileOutputProfile? existingProfile))
        {
            //profileSettings.FileProfiles[request.FileOutputProfileDto.Name] = request.FileOutputProfileDto;
            await messageService.SendError("Profile already exists");
            return APIResponse.ErrorWithMessage("Profile already exists");
        }
        else
        {
            profileSettings.FileProfiles.Add(request.FileOutputProfileDto.Name, request.FileOutputProfileDto);
        }


        Logger.LogInformation("AddFileProfileRequest");

        SettingsHelper.UpdateSetting(profileSettings);

        //DataResponse<SettingDto> ret = await Sender.Send(new GetSettingsRequest(), cancellationToken);
        //return APIResponse.Success(new UpdateSettingResponse { Settings = ret.Data, NeedsLogOut = false });
        await dataRefreshService.RefreshFileProfiles();
        return APIResponse.Ok;
    }

}