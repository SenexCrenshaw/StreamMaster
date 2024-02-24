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
            Name = request.Name,
            Parameters = request.Parameters,
            Timeout = request.TimeOut,
            IsM3U8 = request.IsM3U8
        };

        currentSetting.FFMPEGProfiles.Add(profile);

        Logger.LogInformation("AddFFMPEGProfileRequest");

        FileUtil.UpdateSetting(currentSetting);
        MemoryCache.SetSetting(currentSetting);

        SettingDto ret = Mapper.Map<SettingDto>(currentSetting);
        return new UpdateSettingResponse { Settings = ret, NeedsLogOut = false };
    }

}