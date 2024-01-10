using StreamMaster.Application.Services;
using StreamMaster.SchedulesDirect;

using static StreamMaster.Application.Settings.Commands.UpdateSettingRequestHandler;

namespace StreamMaster.Application.Settings.Commands;

public class UpdateSettingRequest : IRequest<UpdateSettingResponse>
{
    public SDSettingsRequest? SDSettings { get; set; }
    public bool? ShowClientHostNames { get; set; }
    public string? AdminPassword { get; set; }
    public string? AdminUserName { get; set; }
    public string? ApiKey { get; set; }
    public AuthenticationType? AuthenticationMethod { get; set; }
    public bool? CacheIcons { get; set; }
    public bool? CleanURLs { get; set; }
    public string? ClientUserAgent { get; set; }
    public string? DeviceID { get; set; }
    public string? DummyRegex { get; set; }
    public bool? EnableSSL { get; set; }
    public string? FFMPegExecutable { get; set; }
    public string? FFMpegOptions { get; set; }
    public int? GlobalStreamLimit { get; set; }
    public bool? M3UFieldChannelId { get; set; }
    public bool? M3UFieldChannelNumber { get; set; }
    public bool? M3UFieldCUID { get; set; }
    public bool? M3UFieldGroupTitle { get; set; }
    public bool? M3UFieldTvgChno { get; set; }
    public bool? M3UStationId { get; set; }
    public bool? M3UFieldTvgId { get; set; }
    public bool? M3UFieldTvgLogo { get; set; }
    public bool? M3UFieldTvgName { get; set; }
    public bool? M3UUseChnoForId { get; set; }
    public bool? M3UIgnoreEmptyEPGID { get; set; }
    public int? MaxConnectRetry { get; set; }
    public int? MaxConnectRetryTimeMS { get; set; }
    public int? PreloadPercentage { get; set; }
    public int? RingBufferSizeMB { get; set; }
    //public bool? SDEnabled { get; set; }
    //public string? SDCountry { get; set; }
    //public string? SDPassword { get; set; }
    //public string? SDPostalCode { get; set; }
    //public List<StationIdLineup>? SDStationIds { get; set; }
    //public string? SDUserName { get; set; }
    public int? SourceBufferPreBufferPercentage { get; set; }
    public string? SSLCertPassword { get; set; }
    public string? SSLCertPath { get; set; }
    public string? StreamingClientUserAgent { get; set; }
    public StreamingProxyTypes? StreamingProxyType { get; set; }
    public bool? VideoStreamAlwaysUseEPGLogo { get; set; }
    public List<string>? NameRegex { get; set; } = [];
}

public class UpdateSettingRequestHandler(IBackgroundTaskQueue taskQueue, ILogger<UpdateSettingRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateSettingRequest, UpdateSettingResponse>
{
    public static void CopyNonNullFields(SDSettingsRequest source, SDSettings destination)
    {
        if (source == null || destination == null)
        {
            return;
        }

        if (source.SeriesPosterArt != null)
        {
            destination.SeriesPosterArt = (bool)source.SeriesPosterArt;
        }

        if (source.SeriesWsArt != null)
        {
            destination.SeriesWsArt = (bool)source.SeriesWsArt;
        }

        if (source.SeriesPosterAspect != null)
        {
            destination.SeriesPosterAspect = source.SeriesPosterAspect;
        }

        if (source.ArtworkSize != null)
        {
            destination.ArtworkSize = source.ArtworkSize;
        }

        if (source.ExcludeCastAndCrew != null)
        {
            destination.ExcludeCastAndCrew = (bool)source.ExcludeCastAndCrew;
        }

        if (source.AlternateSEFormat != null)
        {
            destination.AlternateSEFormat = (bool)source.AlternateSEFormat;
        }

        if (source.PrefixEpisodeDescription != null)
        {
            destination.PrefixEpisodeDescription = (bool)source.PrefixEpisodeDescription;
        }

        if (source.PrefixEpisodeTitle != null)
        {
            destination.PrefixEpisodeTitle = (bool)source.PrefixEpisodeTitle;
        }

        if (source.AlternateLogoStyle != null)
        {
            destination.AlternateLogoStyle = source.AlternateLogoStyle;
        }

        if (source.PrefixEpisodeTitle != null)
        {
            destination.PrefixEpisodeTitle = (bool)source.PrefixEpisodeTitle;
        }

        if (source.AppendEpisodeDesc != null)
        {
            destination.AppendEpisodeDesc = (bool)source.AppendEpisodeDesc;
        }

        if (source.SDEPGDays != null)
        {
            destination.SDEPGDays = (int)source.SDEPGDays;
        }

        if (source.SDEnabled != null)
        {
            destination.SDEnabled = (bool)source.SDEnabled;
        }

        if (source.SDUserName != null)
        {
            destination.SDUserName = source.SDUserName;
        }

        if (source.SDCountry != null)
        {
            destination.SDCountry = source.SDCountry;
        }

        if (source.SDPassword != null)
        {
            destination.SDPassword = HashHelper.GetSHA1Hash(source.SDPassword);
        }

        if (!string.IsNullOrEmpty(source.SDPostalCode))
        {
            destination.SDPostalCode = source.SDPostalCode;
        }

        if (source.SDStationIds != null)
        {
            destination.SDStationIds = source.SDStationIds;
        }

        if (source.SeasonEventImages != null)
        {
            destination.SeasonEventImages = (bool)source.SeasonEventImages;
        }

        if (source.XmltvAddFillerData != null)
        {
            destination.XmltvAddFillerData = (bool)source.XmltvAddFillerData;
        }

        //if (source.XmltvFillerProgramDescription != null)
        //{
        //    destination.XmltvFillerProgramDescription = source.XmltvFillerProgramDescription;
        //}

        if (source.XmltvFillerProgramLength != null)
        {
            destination.XmltvFillerProgramLength = (int)source.XmltvFillerProgramLength;
        }

        if (source.XmltvIncludeChannelNumbers != null)
        {
            destination.XmltvIncludeChannelNumbers = (bool)source.XmltvIncludeChannelNumbers;
        }

        if (source.XmltvExtendedInfoInTitleDescriptions != null)
        {
            destination.XmltvExtendedInfoInTitleDescriptions = (bool)source.XmltvExtendedInfoInTitleDescriptions;
        }

        if (source.XmltvSingleImage != null)
        {
            destination.XmltvSingleImage = (bool)source.XmltvSingleImage;
        }
    }

    public async Task<UpdateSettingResponse> Handle(UpdateSettingRequest request, CancellationToken cancellationToken)
    {
        Setting currentSetting = await GetSettingsAsync();

        bool needsLogOut = await UpdateSetting(currentSetting, request, cancellationToken);

        Logger.LogInformation("UpdateSettingRequest");
        FileUtil.UpdateSetting(currentSetting);

        MemoryCacheEntryOptions cacheEntryOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };

        _ = memoryCache.Set("Setting", currentSetting, cacheEntryOptions);

        SettingDto ret = Mapper.Map<SettingDto>(currentSetting);
        await HubContext.Clients.All.SettingsUpdate(ret).ConfigureAwait(false);
        if (request.SDSettings?.SDStationIds != null)
        {
            await HubContext.Clients.All.SchedulesDirectsRefresh().ConfigureAwait(false);
        }

        return new UpdateSettingResponse { Settings = ret, NeedsLogOut = needsLogOut };
    }

    /// <summary>
    /// Updates the current setting based on the provided request.
    /// </summary>
    /// <param name="currentSetting">The current setting.</param>
    /// <param name="request">The update setting request.</param>
    /// <returns>The updated setting as a SettingDto object.</returns>
    private async Task<bool> UpdateSetting(Setting currentSetting, UpdateSettingRequest request, CancellationToken cancellationToken)
    {
        bool needsLogOut = false;
        bool needsSetProgrammes = false;
        if (request.CacheIcons != null && request.CacheIcons != currentSetting.CacheIcons)
        {
            currentSetting.CacheIcons = (bool)request.CacheIcons;
        }

        if (request.CleanURLs != null && request.CleanURLs != currentSetting.CleanURLs)
        {
            currentSetting.CleanURLs = (bool)request.CleanURLs;
        }

        if (request.SDSettings != null)
        {
            CopyNonNullFields(request.SDSettings, currentSetting.SDSettings);
        }

        if (request.EnableSSL != null && request.EnableSSL != currentSetting.EnableSSL)
        {
            currentSetting.EnableSSL = (bool)request.EnableSSL;
        }

        if (request.VideoStreamAlwaysUseEPGLogo != null && request.VideoStreamAlwaysUseEPGLogo != currentSetting.VideoStreamAlwaysUseEPGLogo)
        {
            currentSetting.VideoStreamAlwaysUseEPGLogo = (bool)request.VideoStreamAlwaysUseEPGLogo;
        }

        //if (request.EPGAlwaysUseVideoStreamName != null && request.EPGAlwaysUseVideoStreamName != currentSetting.EPGAlwaysUseVideoStreamName)
        //{
        //    currentSetting.EPGAlwaysUseVideoStreamName = (bool)request.EPGAlwaysUseVideoStreamName;
        //}

        if (request.M3UIgnoreEmptyEPGID != null)
        {
            currentSetting.M3UIgnoreEmptyEPGID = (bool)request.M3UIgnoreEmptyEPGID;
        }

        if (request.M3UFieldCUID != null)
        {
            currentSetting.M3UFieldCUID = (bool)request.M3UFieldCUID;
        }

        if (request.M3UUseChnoForId != null)
        {
            currentSetting.M3UUseChnoForId = (bool)request.M3UUseChnoForId;
        }

        if (request.M3UFieldChannelId != null)
        {
            currentSetting.M3UFieldChannelId = (bool)request.M3UFieldChannelId;
        }

        if (request.M3UFieldChannelNumber != null)
        {
            currentSetting.M3UFieldChannelNumber = (bool)request.M3UFieldChannelNumber;
        }

        if (request.M3UFieldTvgName != null)
        {
            currentSetting.M3UFieldTvgName = (bool)request.M3UFieldTvgName;
        }

        if (request.ShowClientHostNames != null)
        {
            currentSetting.ShowClientHostNames = (bool)request.ShowClientHostNames;
        }

        //if (request.SDEnabled != null)
        //{
        //    currentSetting.SDSettings.SDEnabled = (bool)request.SDEnabled;

        //    needsSetProgrammes = request.SDEnabled != null && request.SDEnabled == true;
        //}

        if (request.DummyRegex != null)
        {
            currentSetting.DummyRegex = request.DummyRegex;
        }

        if (request.M3UFieldTvgChno != null)
        {
            currentSetting.M3UFieldTvgChno = (bool)request.M3UFieldTvgChno;
        }

        if (request.M3UStationId != null)
        {
            currentSetting.M3UStationId = (bool)request.M3UStationId;
        }

        if (request.M3UFieldTvgId != null)
        {
            currentSetting.M3UFieldTvgId = (bool)request.M3UFieldTvgId;
        }

        if (request.M3UFieldTvgLogo != null)
        {
            currentSetting.M3UFieldTvgLogo = (bool)request.M3UFieldTvgLogo;
        }

        if (request.M3UFieldGroupTitle != null)
        {
            currentSetting.M3UFieldGroupTitle = (bool)request.M3UFieldGroupTitle;
        }

        if (request.SSLCertPath != null && request.SSLCertPath != currentSetting.SSLCertPath)
        {
            currentSetting.SSLCertPath = request.SSLCertPath;
        }

        if (request.SSLCertPassword != null && request.SSLCertPassword != currentSetting.SSLCertPassword)
        {
            currentSetting.SSLCertPassword = request.SSLCertPassword;
        }

        if (request.ClientUserAgent != null && request.ClientUserAgent != currentSetting.ClientUserAgent)
        {
            currentSetting.ClientUserAgent = request.ClientUserAgent;
        }

        if (request.StreamingClientUserAgent != null && request.StreamingClientUserAgent != currentSetting.StreamingClientUserAgent)
        {
            currentSetting.StreamingClientUserAgent = request.StreamingClientUserAgent;
        }

        if (!string.IsNullOrEmpty(request.ApiKey) && request.ApiKey != currentSetting.ApiKey)
        {
            currentSetting.ApiKey = request.ApiKey;
        }

        if (request.AdminPassword != null && request.AdminPassword != currentSetting.AdminPassword)
        {
            currentSetting.AdminPassword = request.AdminPassword;
            needsLogOut = true;
        }

        if (request.AdminUserName != null && request.AdminUserName != currentSetting.AdminUserName)
        {
            currentSetting.AdminUserName = request.AdminUserName;
            needsLogOut = true;
        }

        if (!string.IsNullOrEmpty(request.DeviceID) && request.DeviceID != currentSetting.DeviceID)
        {
            currentSetting.DeviceID = request.DeviceID;
        }

        if (!string.IsNullOrEmpty(request.FFMPegExecutable) && request.FFMPegExecutable != currentSetting.FFMPegExecutable)
        {
            currentSetting.FFMPegExecutable = request.FFMPegExecutable;
        }

        if (!string.IsNullOrEmpty(request.FFMpegOptions) && request.FFMpegOptions != currentSetting.FFMpegOptions)
        {
            currentSetting.FFMpegOptions = request.FFMpegOptions;
        }


        if (request.MaxConnectRetry != null && request.MaxConnectRetry >= 0 && request.MaxConnectRetry != currentSetting.MaxConnectRetry)
        {
            currentSetting.MaxConnectRetry = (int)request.MaxConnectRetry;
        }

        if (request.MaxConnectRetryTimeMS != null && request.MaxConnectRetryTimeMS >= 0 && request.MaxConnectRetryTimeMS != currentSetting.MaxConnectRetryTimeMS)
        {
            currentSetting.MaxConnectRetryTimeMS = (int)request.MaxConnectRetryTimeMS;
        }

        if (request.GlobalStreamLimit != null && request.GlobalStreamLimit >= 0 && request.GlobalStreamLimit != currentSetting.GlobalStreamLimit)
        {
            currentSetting.GlobalStreamLimit = (int)request.GlobalStreamLimit;
        }

        if (request.PreloadPercentage != null && request.PreloadPercentage >= 0 && request.PreloadPercentage <= 100 && request.PreloadPercentage != currentSetting.PreloadPercentage)
        {
            currentSetting.PreloadPercentage = (int)request.PreloadPercentage;
        }

        if (request.RingBufferSizeMB != null && request.RingBufferSizeMB >= 0 && request.RingBufferSizeMB != currentSetting.RingBufferSizeMB)
        {
            currentSetting.RingBufferSizeMB = (int)request.RingBufferSizeMB;
        }



        if (request.NameRegex != null)
        {
            currentSetting.NameRegex = request.NameRegex;
        }

        if (request.StreamingProxyType != null && request.StreamingProxyType != currentSetting.StreamingProxyType)
        {
            currentSetting.StreamingProxyType = (StreamingProxyTypes)request.StreamingProxyType;
        }

        if (request.AuthenticationMethod != null && request.AuthenticationMethod != currentSetting.AuthenticationMethod)
        {
            needsLogOut = true;
            currentSetting.AuthenticationMethod = (AuthenticationType)request.AuthenticationMethod;
        }

        if (needsSetProgrammes)
        {
            await taskQueue.EPGSync(cancellationToken).ConfigureAwait(false);
        }

        return needsLogOut;
    }

    public class UpdateSettingResponse
    {
        public bool NeedsLogOut { get; set; }
        public SettingDto Settings { get; set; }
    }
}