
namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddOutputProfileRequest(OutputProfileDto OutputProfileDto) : IRequest<APIResponse>;

public class AddOutputProfileRequestHandler(ILogger<AddOutputProfileRequest> Logger,
    IMessageService messageService,
    IOptionsMonitor<OutputProfiles> intProfileSettings, IDataRefreshService dataRefreshService)
: IRequestHandler<AddOutputProfileRequest, APIResponse>
{


    public async Task<APIResponse> Handle(AddOutputProfileRequest request, CancellationToken cancellationToken)
    {
        OutputProfiles profileSettings = intProfileSettings.CurrentValue;

        List<string> badNames = profileSettings.OutProfiles
            .Where(kvp => kvp.Value.IsReadOnly)
            .Select(kvp => kvp.Key)
            .ToList();

        if (badNames.Contains(request.OutputProfileDto.Name, StringComparer.OrdinalIgnoreCase))
        {
            return APIResponse.ErrorWithMessage($"Cannot use name {request.OutputProfileDto.Name}");
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