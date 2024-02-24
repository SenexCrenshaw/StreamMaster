namespace StreamMaster.Application.Settings.Commands;

public record AddFFMPEGProfileRequest(string Name, string Parameters, int TimeOut, bool IsM3U8) : IRequest<UpdateSettingResponse> { }

public class AddFFMPEGProfileRequestHandler(ILogger<AddFFMPEGProfileRequest> Logger, IMapper Mapper, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext, IMemoryCache MemoryCache)
: IRequestHandler<AddFFMPEGProfileRequest, UpdateSettingResponse>
{
    public async Task<UpdateSettingResponse> Handle(AddFFMPEGProfileRequest request, CancellationToken cancellationToken)
    {
        Setting currentSetting = MemoryCache.GetSetting();

        FFMPEGProfile profile = new()
        {
            Parameters = request.Parameters,
            Timeout = request.TimeOut,
            IsM3U8 = request.IsM3U8
        };

        if (currentSetting.FFMPEGProfiles.TryGetValue(request.Name, out FFMPEGProfile? existingProfile))
        {
            currentSetting.FFMPEGProfiles[request.Name] = profile;
        }
        else
        {
            currentSetting.FFMPEGProfiles.Add(request.Name, profile);
        }


        Logger.LogInformation("AddFFMPEGProfileRequest");

        FileUtil.UpdateSetting(currentSetting);
        MemoryCache.SetSetting(currentSetting);

        SettingDto ret = Mapper.Map<SettingDto>(currentSetting);
        return new UpdateSettingResponse { Settings = ret, NeedsLogOut = false };
    }

}