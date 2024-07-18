
namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddOutputProfileRequest(OutputProfileDto OutputProfileDto) : IRequest<APIResponse>;

public class AddOutputProfileRequestHandler(ILogger<AddOutputProfileRequest> Logger,
    IMessageService messageService,
    IOptionsMonitor<OutputProfiles> intprofilesettings, IDataRefreshService dataRefreshService)
: IRequestHandler<AddOutputProfileRequest, APIResponse>
{

    private readonly OutputProfiles profileSettings = intprofilesettings.CurrentValue;

    public async Task<APIResponse> Handle(AddOutputProfileRequest request, CancellationToken cancellationToken)
    {

        if (request.OutputProfileDto.Name.Equals("default", StringComparison.OrdinalIgnoreCase))
        {
            return APIResponse.ErrorWithMessage("Cannot use name default");
        }

        if (profileSettings.OutProfiles.TryGetValue(request.OutputProfileDto.Name, out _))
        {
            //profileSettings.FileProfiles[request.OutputProfileDto.Name] = request.OutputProfileDto;
            await messageService.SendError("Profile already exists");
            return APIResponse.ErrorWithMessage("Profile already exists");
        }
        else
        {
            profileSettings.OutProfiles.Add(request.OutputProfileDto.Name, request.OutputProfileDto);
        }


        Logger.LogInformation("AddOutputProfileRequest");

        SettingsHelper.UpdateSetting(profileSettings);

        //DataResponse<SettingDto> ret = await Sender.Send(new GetSettingsRequest(), cancellationToken);
        //return APIResponse.Success(new UpdateSettingResponse { Settings = ret.Data, NeedsLogOut = false });
        await dataRefreshService.RefreshOutputProfiles();
        return APIResponse.Ok;
    }

}