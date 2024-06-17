
namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddVideoProfileRequest(string Name, string Parameters, int TimeOut, bool IsM3U8) : IRequest<APIResponse> { }

public class AddVideoVideoProfileRequestHandler(ILogger<AddVideoProfileRequest> Logger, ISender Sender, IOptionsMonitor<VideoOutputProfiles> intprofilesettings, IMapper Mapper)
: IRequestHandler<AddVideoProfileRequest, APIResponse>
{

    private readonly VideoOutputProfiles profileSettings = intprofilesettings.CurrentValue;

    public async Task<APIResponse> Handle(AddVideoProfileRequest request, CancellationToken cancellationToken)
    {

        VideoOutputProfile profile = new()
        {
            Parameters = request.Parameters,
            Timeout = request.TimeOut,
            IsM3U8 = request.IsM3U8
        };

        if (profileSettings.VideoProfiles.TryGetValue(request.Name, out VideoOutputProfile? existingProfile))
        {
            profileSettings.VideoProfiles[request.Name] = profile;
        }
        else
        {
            profileSettings.VideoProfiles.Add(request.Name, profile);
        }


        Logger.LogInformation("AddVideoProfileRequest");

        SettingsHelper.UpdateSetting(profileSettings);

        DataResponse<SettingDto> ret = await Sender.Send(new GetSettingsRequest(), cancellationToken);
        //return APIResponse.Success(new UpdateSettingResponse { Settings = ret.Data, NeedsLogOut = false });
        return APIResponse.Ok;
    }

}