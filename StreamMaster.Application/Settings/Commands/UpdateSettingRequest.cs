using StreamMaster.Application.Services;
using StreamMaster.SchedulesDirect;

namespace StreamMaster.Application.Settings.Commands;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class UpdateSettingParameters
{
    public bool? AutoSetEPG { get; set; }
    public bool? BackupEnabled { get; set; }
    public int? BackupVersionsToKeep { get; set; }
    public int? BackupInterval { get; set; }
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

    public bool? PrettyEPG { get; set; }
    public int? MaxConnectRetry { get; set; }
    public int? MaxConnectRetryTimeMS { get; set; }
    public string? SSLCertPassword { get; set; }
    public string? SSLCertPath { get; set; }
    public string? StreamingClientUserAgent { get; set; }
    public string? StreamingProxyType { get; set; }
    public bool? VideoStreamAlwaysUseEPGLogo { get; set; }
    public bool? EnablePrometheus { get; set; }
    public int? MaxLogFiles { get; set; }
    public int? MaxLogFileSizeMB { get; set; }
    public List<string>? NameRegex { get; set; } = [];
}

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateSettingRequest(UpdateSettingParameters parameters) : IRequest<UpdateSettingResponse> { }


public partial class UpdateSettingRequestHandler(
    IBackgroundTaskQueue taskQueue,
    IOptionsMonitor<SDSettings> intsdsettings,
    ILogger<UpdateSettingRequest> Logger,
    IMapper Mapper,
    IDataRefreshService dataRefreshService,
    IOptionsMonitor<Setting> intsettings
    )
: IRequestHandler<UpdateSettingRequest, UpdateSettingResponse>
{
    private readonly Setting settings = intsettings.CurrentValue;
    private readonly SDSettings sdsettings = intsdsettings.CurrentValue;

    public static void CopyNonNullFields(SDSettingsRequest source, SDSettings destination)
    {
        if (source == null || destination == null)
        {
            return;
        }

        if (source.SeriesPosterArt.HasValue)
        {
            destination.SeriesPosterArt = source.SeriesPosterArt.HasValue;
        }

        if (source.SeriesWsArt.HasValue)
        {
            destination.SeriesWsArt = source.SeriesWsArt.Value;
        }

        if (source.SeriesPosterAspect != null)
        {
            destination.SeriesPosterAspect = source.SeriesPosterAspect;
        }



        if (source.ArtworkSize != null)
        {
            destination.ArtworkSize = source.ArtworkSize;
        }

        if (source.ExcludeCastAndCrew.HasValue)
        {
            destination.ExcludeCastAndCrew = source.ExcludeCastAndCrew.Value;
        }

        if (source.AlternateSEFormat.HasValue)
        {
            destination.AlternateSEFormat = source.AlternateSEFormat.Value;
        }

        if (source.PrefixEpisodeDescription != null)
        {
            destination.PrefixEpisodeDescription = (bool)source.PrefixEpisodeDescription;
        }

        if (source.PrefixEpisodeTitle.HasValue)
        {
            destination.PrefixEpisodeTitle = source.PrefixEpisodeTitle.Value;
        }

        if (source.AlternateLogoStyle != null)
        {
            destination.AlternateLogoStyle = source.AlternateLogoStyle;
        }

        if (source.PrefixEpisodeTitle.HasValue)
        {
            destination.PrefixEpisodeTitle = source.PrefixEpisodeTitle.Value;
        }

        if (source.AppendEpisodeDesc.HasValue)
        {
            destination.AppendEpisodeDesc = source.AppendEpisodeDesc.Value;
        }

        if (source.SDEPGDays.HasValue)
        {
            destination.SDEPGDays = source.SDEPGDays.Value;
        }

        if (source.SDEnabled.HasValue)
        {
            destination.SDEnabled = source.SDEnabled.Value;
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
            destination.SDPassword = source.SDPassword.GetSHA1Hash();
        }

        if (!string.IsNullOrEmpty(source.SDPostalCode))
        {
            destination.SDPostalCode = source.SDPostalCode;
        }

        if (source.SDStationIds != null)
        {
            destination.SDStationIds = source.SDStationIds;
        }

        if (source.SeasonEventImages.HasValue)
        {
            destination.SeasonEventImages = source.SeasonEventImages.Value;
        }

        if (source.XmltvAddFillerData.HasValue)
        {
            destination.XmltvAddFillerData = source.XmltvAddFillerData.Value;
        }

        if (source.XmltvFillerProgramLength.HasValue)
        {
            destination.XmltvFillerProgramLength = source.XmltvFillerProgramLength.Value;
        }

        if (source.MaxSubscribedLineups.HasValue)
        {
            destination.MaxSubscribedLineups = source.MaxSubscribedLineups.Value;
        }

        if (source.XmltvIncludeChannelNumbers.HasValue)
        {
            destination.XmltvIncludeChannelNumbers = source.XmltvIncludeChannelNumbers.Value;
        }

        if (source.XmltvExtendedInfoInTitleDescriptions.HasValue)
        {
            destination.XmltvExtendedInfoInTitleDescriptions = source.XmltvExtendedInfoInTitleDescriptions.Value;
        }

        if (source.XmltvSingleImage.HasValue)
        {
            destination.XmltvSingleImage = source.XmltvSingleImage.Value;
        }
    }

    public async Task<UpdateSettingResponse> Handle(UpdateSettingRequest request, CancellationToken cancellationToken)
    {
        Setting currentSetting = settings;

        bool needsLogOut = await UpdateSetting(currentSetting, sdsettings, request, cancellationToken);

        Logger.LogInformation("UpdateSettingRequest");
        SettingsHelper.UpdateSetting(currentSetting);

        SettingDto ret = Mapper.Map<SettingDto>(currentSetting);
        //await HubContext.Clients.All.SettingsUpdate(ret).ConfigureAwait(false);
        //if (request.parameters.SDSettings?.SDStationIds != null)
        //{
        //    await HubContext.Clients.All.SchedulesDirectsRefresh().ConfigureAwait(false);
        //}

        return new UpdateSettingResponse { Settings = ret, NeedsLogOut = needsLogOut };
    }

    /// <summary>
    /// Updates the current setting based on the provided request.parameters.
    /// </summary>
    /// <param name="currentSetting">The current setting.</param>
    /// <param name="request">The update setting request.parameters.</param>
    /// <returns>The updated setting as a SettingDto object.</returns>
    private async Task<bool> UpdateSetting(Setting currentSetting, SDSettings sdsettings, UpdateSettingRequest request, CancellationToken cancellationToken)
    {
        bool needsLogOut = false;
        bool needsSetProgrammes = false;
        if (request.parameters.CacheIcons.HasValue)
        {
            currentSetting.CacheIcons = request.parameters.CacheIcons.Value;
        }

        if (request.parameters.CleanURLs.HasValue)
        {
            currentSetting.CleanURLs = request.parameters.CleanURLs.Value;
        }

        if (request.parameters.SDSettings != null)
        {
            CopyNonNullFields(request.parameters.SDSettings, sdsettings);
            SettingsHelper.UpdateSetting(sdsettings);
        }

        if (request.parameters.EnableSSL.HasValue)
        {
            currentSetting.EnableSSL = request.parameters.EnableSSL.Value;
        }

        if (request.parameters.VideoStreamAlwaysUseEPGLogo.HasValue)
        {
            currentSetting.VideoStreamAlwaysUseEPGLogo = request.parameters.VideoStreamAlwaysUseEPGLogo.Value;
        }

        if (request.parameters.PrettyEPG.HasValue)
        {
            currentSetting.PrettyEPG = request.parameters.PrettyEPG.Value;
        }

        //if (request.parameters.M3UIgnoreEmptyEPGID != null)
        //{
        //    currentSetting.M3UIgnoreEmptyEPGID = (bool)request.parameters.M3UIgnoreEmptyEPGID;
        //}

        //if (request.parameters.M3UUseCUIDForChannelID != null)
        //{
        //    currentSetting.M3UUseCUIDForChannelID = (bool)request.parameters.M3UUseCUIDForChannelID;
        //}

        if (request.parameters.MaxLogFiles.HasValue)
        {
            currentSetting.MaxLogFiles = request.parameters.MaxLogFiles.Value;
        }

        if (request.parameters.MaxLogFileSizeMB.HasValue)
        {
            currentSetting.MaxLogFileSizeMB = request.parameters.MaxLogFileSizeMB.Value;
        }

        if (request.parameters.EnablePrometheus.HasValue)
        {
            currentSetting.EnablePrometheus = request.parameters.EnablePrometheus.Value;
        }

        if (request.parameters.MaxLogFiles.HasValue)
        {
            currentSetting.MaxLogFiles = request.parameters.MaxLogFiles.Value;
        }
        //if (request.parameters.M3UUseChnoForId != null)
        //{
        //    currentSetting.M3UUseChnoForId = (bool)request.parameters.M3UUseChnoForId;
        //}

        if (request.parameters.BackupEnabled.HasValue)
        {
            currentSetting.BackupEnabled = request.parameters.BackupEnabled.Value;
        }

        if (request.parameters.AutoSetEPG.HasValue)
        {
            currentSetting.AutoSetEPG = request.parameters.AutoSetEPG.Value;
        }


        if (request.parameters.BackupVersionsToKeep.HasValue)
        {
            currentSetting.BackupVersionsToKeep = request.parameters.BackupVersionsToKeep.Value;
        }

        if (request.parameters.BackupInterval.HasValue)
        {
            currentSetting.BackupInterval = request.parameters.BackupInterval.Value;
        }

        if (request.parameters.ShowClientHostNames.HasValue)
        {
            currentSetting.ShowClientHostNames = request.parameters.ShowClientHostNames.Value;
        }

        if (request.parameters.DummyRegex != null)
        {
            currentSetting.DummyRegex = request.parameters.DummyRegex;
        }

        //if (request.parameters.M3UStationId != null)
        //{
        //    currentSetting.M3UStationId = (bool)request.parameters.M3UStationId;
        //}

        //if (request.parameters.M3UFieldGroupTitle != null)
        //{
        //    currentSetting.M3UFieldGroupTitle = (bool)request.parameters.M3UFieldGroupTitle;
        //}

        if (request.parameters.SSLCertPath != null && request.parameters.SSLCertPath != currentSetting.SSLCertPath)
        {
            currentSetting.SSLCertPath = request.parameters.SSLCertPath;
        }

        if (request.parameters.SSLCertPassword != null && request.parameters.SSLCertPassword != currentSetting.SSLCertPassword)
        {
            currentSetting.SSLCertPassword = request.parameters.SSLCertPassword;
        }

        if (request.parameters.ClientUserAgent != null && request.parameters.ClientUserAgent != currentSetting.ClientUserAgent)
        {
            currentSetting.ClientUserAgent = request.parameters.ClientUserAgent;
        }

        if (request.parameters.StreamingClientUserAgent != null && request.parameters.StreamingClientUserAgent != currentSetting.StreamingClientUserAgent)
        {
            currentSetting.StreamingClientUserAgent = request.parameters.StreamingClientUserAgent;
        }

        if (!string.IsNullOrEmpty(request.parameters.ApiKey) && request.parameters.ApiKey != currentSetting.ApiKey)
        {
            currentSetting.ApiKey = request.parameters.ApiKey;
        }

        if (request.parameters.AdminPassword != null && request.parameters.AdminPassword != currentSetting.AdminPassword)
        {
            currentSetting.AdminPassword = request.parameters.AdminPassword;
            needsLogOut = true;
        }

        if (request.parameters.AdminUserName != null && request.parameters.AdminUserName != currentSetting.AdminUserName)
        {
            currentSetting.AdminUserName = request.parameters.AdminUserName;
            needsLogOut = true;
        }

        if (!string.IsNullOrEmpty(request.parameters.DeviceID) && request.parameters.DeviceID != currentSetting.DeviceID)
        {
            currentSetting.DeviceID = request.parameters.DeviceID;
        }

        if (!string.IsNullOrEmpty(request.parameters.FFMPegExecutable) && request.parameters.FFMPegExecutable != currentSetting.FFMPegExecutable)
        {
            currentSetting.FFMPegExecutable = request.parameters.FFMPegExecutable;
        }

        //if (!string.IsNullOrEmpty(request.parameters.FFMpegOptions) && request.parameters.FFMpegOptions != currentSetting.FFMpegOptions)
        //{
        //    currentSetting.FFMpegOptions = request.parameters.FFMpegOptions;
        //}


        if (request.parameters.MaxConnectRetry != null && request.parameters.MaxConnectRetry >= 0 && request.parameters.MaxConnectRetry != currentSetting.MaxConnectRetry)
        {
            currentSetting.MaxConnectRetry = (int)request.parameters.MaxConnectRetry;
        }

        if (request.parameters.MaxConnectRetryTimeMS != null && request.parameters.MaxConnectRetryTimeMS >= 0 && request.parameters.MaxConnectRetryTimeMS != currentSetting.MaxConnectRetryTimeMS)
        {
            currentSetting.MaxConnectRetryTimeMS = (int)request.parameters.MaxConnectRetryTimeMS;
        }

        if (request.parameters.GlobalStreamLimit != null && request.parameters.GlobalStreamLimit >= 0 && request.parameters.GlobalStreamLimit != currentSetting.GlobalStreamLimit)
        {
            currentSetting.GlobalStreamLimit = (int)request.parameters.GlobalStreamLimit;
        }

        if (request.parameters.NameRegex != null)
        {
            currentSetting.NameRegex = request.parameters.NameRegex;
        }

        if (request.parameters.StreamingProxyType != null && request.parameters.StreamingProxyType != currentSetting.StreamingProxyType)
        {
            currentSetting.StreamingProxyType = request.parameters.StreamingProxyType;
        }

        if (request.parameters.AuthenticationMethod != null && request.parameters.AuthenticationMethod != currentSetting.AuthenticationMethod)
        {
            needsLogOut = true;
            currentSetting.AuthenticationMethod = (AuthenticationType)request.parameters.AuthenticationMethod;
        }

        if (needsSetProgrammes)
        {
            await taskQueue.EPGSync(cancellationToken).ConfigureAwait(false);
        }

        return needsLogOut;
    }
}