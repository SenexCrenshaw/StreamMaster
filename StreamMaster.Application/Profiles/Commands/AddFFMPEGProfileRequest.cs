
namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddFFMPEGProfileRequest(string Name, string Parameters, int TimeOut, bool IsM3U8) : IRequest<APIResponse> { }

public class AddFFMPEGProfileRequestHandler(ILogger<AddFFMPEGProfileRequest> Logger, ISender Sender, IOptionsMonitor<FFMPEGProfiles> intprofilesettings, IMapper Mapper)
: IRequestHandler<AddFFMPEGProfileRequest, APIResponse>
{

    private readonly FFMPEGProfiles profileSettings = intprofilesettings.CurrentValue;

    public async Task<APIResponse> Handle(AddFFMPEGProfileRequest request, CancellationToken cancellationToken)
    {

        FFMPEGProfile profile = new()
        {
            Parameters = request.Parameters,
            Timeout = request.TimeOut,
            IsM3U8 = request.IsM3U8
        };

        if (profileSettings.Profiles.TryGetValue(request.Name, out FFMPEGProfile? existingProfile))
        {
            profileSettings.Profiles[request.Name] = profile;
        }
        else
        {
            profileSettings.Profiles.Add(request.Name, profile);
        }


        Logger.LogInformation("AddFFMPEGProfileRequest");

        SettingsHelper.UpdateSetting(profileSettings);

        DataResponse<SettingDto> ret = await Sender.Send(new GetSettingsRequest(), cancellationToken);
        //return APIResponse.Success(new UpdateSettingResponse { Settings = ret.Data, NeedsLogOut = false });
        return APIResponse.Ok;
    }

}