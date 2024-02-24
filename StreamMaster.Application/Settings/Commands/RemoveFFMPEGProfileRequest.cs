namespace StreamMaster.Application.Settings.Commands;

public record RemoveFFMPEGProfileRequest(string Name) : IRequest<UpdateSettingResponse> { }

public class RemoveFFMPEGProfileRequestHandler(ILogger<RemoveFFMPEGProfileRequest> Logger, IMapper Mapper, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext, IMemoryCache MemoryCache)
: IRequestHandler<RemoveFFMPEGProfileRequest, UpdateSettingResponse>
{
    public async Task<UpdateSettingResponse> Handle(RemoveFFMPEGProfileRequest request, CancellationToken cancellationToken)
    {
        Setting currentSetting = MemoryCache.GetSetting();

        if (currentSetting.FFMPEGProfiles.TryGetValue(request.Name, out FFMPEGProfile? profile))
        {
            currentSetting.FFMPEGProfiles.Remove(request.Name);

            Logger.LogInformation("RemoveFFMPEGProfileRequest");

            FileUtil.UpdateSetting(currentSetting);
            MemoryCache.SetSetting(currentSetting);
        }

        SettingDto retNull = Mapper.Map<SettingDto>(currentSetting);
        return new UpdateSettingResponse { Settings = retNull, NeedsLogOut = false };


    }

}