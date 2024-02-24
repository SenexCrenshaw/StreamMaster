namespace StreamMaster.Application.Settings.Commands;

public record RemoveFFMPEGProfileRequest(string Id) : IRequest<UpdateSettingResponse> { }

public class RemoveFFMPEGProfileRequestHandler(ILogger<RemoveFFMPEGProfileRequest> Logger, IMapper Mapper, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext, IMemoryCache MemoryCache)
: IRequestHandler<RemoveFFMPEGProfileRequest, UpdateSettingResponse>
{
    public async Task<UpdateSettingResponse> Handle(RemoveFFMPEGProfileRequest request, CancellationToken cancellationToken)
    {
        Setting currentSetting = MemoryCache.GetSetting();


        FFMPEGProfile? profile = currentSetting.FFMPEGProfiles.FirstOrDefault(x => x.Id == request.Id);
        if (profile == null)
        {
            SettingDto retNull = Mapper.Map<SettingDto>(currentSetting);
            return new UpdateSettingResponse { Settings = retNull, NeedsLogOut = false };
        }

        currentSetting.FFMPEGProfiles.Add(profile);

        Logger.LogInformation("RemoveFFMPEGProfileRequest");

        FileUtil.UpdateSetting(currentSetting);
        MemoryCache.SetSetting(currentSetting);

        SettingDto ret = Mapper.Map<SettingDto>(currentSetting);
        return new UpdateSettingResponse { Settings = ret, NeedsLogOut = false };
    }

}