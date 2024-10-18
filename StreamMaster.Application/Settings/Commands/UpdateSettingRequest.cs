using StreamMaster.Application.Services;
using StreamMaster.SchedulesDirect;

namespace StreamMaster.Application.Settings.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateSettingRequest(UpdateSettingParameters Parameters) : IRequest<UpdateSettingResponse>;

public partial class UpdateSettingRequestHandler(
    IOptionsMonitor<SDSettings> intsdsettings,
    ILogoService logoService,
    ILogger<UpdateSettingRequest> Logger,
    IMapper Mapper,
    IBackgroundTaskQueue backgroundTaskQueue,
    IOptionsMonitor<Setting> intSettings
) : IRequestHandler<UpdateSettingRequest, UpdateSettingResponse>
{
    private readonly Setting settings = intSettings.CurrentValue;
    private readonly SDSettings sdSettings = intsdsettings.CurrentValue;

    public async Task<UpdateSettingResponse> Handle(UpdateSettingRequest request, CancellationToken cancellationToken)
    {
        Setting currentSetting = settings;
        bool needsLogOut = await UpdateSetting(currentSetting, sdSettings, request);

        Logger.LogInformation("UpdateSettingRequest");
        SettingsHelper.UpdateSetting(currentSetting);

        SettingDto ret = Mapper.Map<SettingDto>(currentSetting);

        return new UpdateSettingResponse { Settings = ret, NeedsLogOut = needsLogOut };
    }

    private static void CopySDNonNullFields(SDSettingsRequest source, SDSettings destination)
    {
        if (source == null || destination == null)
        {
            return;
        }

        if (source.PreferredLogoStyle != null)
        {
            destination.PreferredLogoStyle = source.PreferredLogoStyle;
        }

        if (source.AlternateLogoStyle != null)
        {
            destination.AlternateLogoStyle = source.AlternateLogoStyle;
        }

        if (source.SeriesPosterArt.HasValue)
        {
            destination.SeriesPosterArt = source.SeriesPosterArt.Value;
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

        if (source.PrefixEpisodeDescription.HasValue)
        {
            destination.PrefixEpisodeDescription = source.PrefixEpisodeDescription.Value;
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

        if (source.SDPostalCode != null)
        {
            destination.SDPostalCode = source.SDPostalCode;
        }

        if (source.HeadendsToView != null)
        {
            destination.HeadendsToView = new List<HeadendToView>(source.HeadendsToView);
        }

        if (source.SDStationIds != null)
        {
            destination.SDStationIds = new List<StationIdLineup>(source.SDStationIds);
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

    private async Task<bool> UpdateSetting(Setting currentSetting, SDSettings sdsettings, UpdateSettingRequest request)
    {
        bool needsLogOut = false;

        if (request.Parameters.LogoCache != null)
        {
            currentSetting.LogoCache = request.Parameters.LogoCache.ToLowerInvariant() switch
            {
                "redirect" => "Redirect",
                "cache" => "Cache",
                _ => "None",
            };
            await logoService.BuildLogosCacheFromSMStreamsAsync(CancellationToken.None);
        }

        if (request.Parameters.CleanURLs.HasValue)
        {
            currentSetting.CleanURLs = request.Parameters.CleanURLs.Value;
        }

        if (request.Parameters.SDSettings != null)
        {
            CopySDNonNullFields(request.Parameters.SDSettings, sdsettings);
            SettingsHelper.UpdateSetting(sdsettings);
        }

        if (request.Parameters.EnableSSL.HasValue)
        {
            currentSetting.EnableSSL = request.Parameters.EnableSSL.Value;
        }

        if (request.Parameters.ShowMessageVideos.HasValue)
        {
            currentSetting.ShowMessageVideos = request.Parameters.ShowMessageVideos.Value;
        }

        if (!string.IsNullOrEmpty(request.Parameters.DefaultCompression))
        {
            string[] validCompressions = ["none", "gz", "zip"];
            currentSetting.DefaultCompression = validCompressions.Contains(request.Parameters.DefaultCompression.ToLower())
                ? request.Parameters.DefaultCompression
                : "gz";
        }

        if (!string.IsNullOrEmpty(request.Parameters.M3U8OutPutProfile))
        {
            currentSetting.M3U8OutPutProfile = request.Parameters.M3U8OutPutProfile;
        }

        if (request.Parameters.PrettyEPG.HasValue)
        {
            currentSetting.PrettyEPG = request.Parameters.PrettyEPG.Value;
        }

        if (!string.IsNullOrEmpty(request.Parameters.ShowIntros))
        {
            currentSetting.ShowIntros = request.Parameters.ShowIntros;
        }

        if (request.Parameters.MaxLogFiles.HasValue)
        {
            currentSetting.MaxLogFiles = request.Parameters.MaxLogFiles.Value;
        }

        if (request.Parameters.MaxLogFileSizeMB.HasValue)
        {
            currentSetting.MaxLogFileSizeMB = request.Parameters.MaxLogFileSizeMB.Value;
        }

        if (request.Parameters.ClientReadTimeOutSeconds.HasValue)
        {
            currentSetting.ClientReadTimeOutSeconds = request.Parameters.ClientReadTimeOutSeconds.Value;
        }

        if (request.Parameters.BackupEnabled.HasValue)
        {
            currentSetting.BackupEnabled = request.Parameters.BackupEnabled.Value;
        }

        if (request.Parameters.ShutDownDelay.HasValue)
        {
            currentSetting.ShutDownDelay = request.Parameters.ShutDownDelay.Value;
        }

        if (request.Parameters.AutoSetEPG.HasValue)
        {
            currentSetting.AutoSetEPG = request.Parameters.AutoSetEPG.Value;
        }

        if (request.Parameters.BackupVersionsToKeep.HasValue)
        {
            currentSetting.BackupVersionsToKeep = request.Parameters.BackupVersionsToKeep.Value;
        }

        if (request.Parameters.BackupInterval.HasValue)
        {
            currentSetting.BackupInterval = request.Parameters.BackupInterval.Value;
        }

        if (request.Parameters.IconCacheExpirationDays.HasValue)
        {
            currentSetting.IconCacheExpirationDays = request.Parameters.IconCacheExpirationDays.Value;
        }

        if (request.Parameters.ShowClientHostNames.HasValue)
        {
            currentSetting.ShowClientHostNames = request.Parameters.ShowClientHostNames.Value;
        }

        if (request.Parameters.DummyRegex != null)
        {
            currentSetting.DummyRegex = request.Parameters.DummyRegex;
        }

        if (request.Parameters.SSLCertPath != null && request.Parameters.SSLCertPath != currentSetting.SSLCertPath)
        {
            currentSetting.SSLCertPath = request.Parameters.SSLCertPath;
        }

        if (!string.IsNullOrEmpty(request.Parameters.DefaultCommandProfileName) && request.Parameters.DefaultCommandProfileName != currentSetting.DefaultCommandProfileName)
        {
            currentSetting.DefaultCommandProfileName = request.Parameters.DefaultCommandProfileName;
        }

        if (!string.IsNullOrEmpty(request.Parameters.DefaultOutputProfileName) && request.Parameters.DefaultOutputProfileName != currentSetting.DefaultOutputProfileName)
        {
            currentSetting.DefaultOutputProfileName = request.Parameters.DefaultOutputProfileName;
        }

        if (request.Parameters.SSLCertPassword != null && request.Parameters.SSLCertPassword != currentSetting.SSLCertPassword)
        {
            currentSetting.SSLCertPassword = request.Parameters.SSLCertPassword;
        }

        if (request.Parameters.ClientUserAgent != null && request.Parameters.ClientUserAgent != currentSetting.ClientUserAgent)
        {
            currentSetting.ClientUserAgent = request.Parameters.ClientUserAgent;
        }

        if (request.Parameters.AdminPassword != null && request.Parameters.AdminPassword != currentSetting.AdminPassword)
        {
            currentSetting.AdminPassword = request.Parameters.AdminPassword;
            needsLogOut = true;
        }

        if (request.Parameters.AdminUserName != null && request.Parameters.AdminUserName != currentSetting.AdminUserName)
        {
            currentSetting.AdminUserName = request.Parameters.AdminUserName;
            needsLogOut = true;
        }

        if (!string.IsNullOrEmpty(request.Parameters.DeviceID) && request.Parameters.DeviceID != currentSetting.DeviceID)
        {
            currentSetting.DeviceID = request.Parameters.DeviceID;
        }

        if (!string.IsNullOrEmpty(request.Parameters.FFMPegExecutable) && request.Parameters.FFMPegExecutable != currentSetting.FFMPegExecutable)
        {
            currentSetting.FFMPegExecutable = request.Parameters.FFMPegExecutable;
        }

        if (!string.IsNullOrEmpty(request.Parameters.FFProbeExecutable) && request.Parameters.FFProbeExecutable != currentSetting.FFProbeExecutable)
        {
            currentSetting.FFProbeExecutable = request.Parameters.FFProbeExecutable;
        }

        if (request.Parameters.MaxConnectRetry.HasValue && request.Parameters.MaxConnectRetry >= 0)
        {
            currentSetting.MaxConnectRetry = request.Parameters.MaxConnectRetry.Value;
        }

        if (request.Parameters.MaxConnectRetryTimeMS.HasValue && request.Parameters.MaxConnectRetryTimeMS >= 0)
        {
            currentSetting.MaxConnectRetryTimeMS = request.Parameters.MaxConnectRetryTimeMS.Value;
        }

        if (request.Parameters.GlobalStreamLimit.HasValue && request.Parameters.GlobalStreamLimit >= 0)
        {
            currentSetting.GlobalStreamLimit = request.Parameters.GlobalStreamLimit.Value;
        }

        if (request.Parameters.NameRegex != null)
        {
            currentSetting.NameRegex = request.Parameters.NameRegex;
        }

        if (request.Parameters.AuthenticationMethod != null && request.Parameters.AuthenticationMethod != currentSetting.AuthenticationMethod)
        {
            needsLogOut = true;
            currentSetting.AuthenticationMethod = request.Parameters.AuthenticationMethod;
        }

        if (request.Parameters.SDSettings?.SDEnabled.HasValue == true)
        {
            await backgroundTaskQueue.EPGSync().ConfigureAwait(false);
        }

        return needsLogOut;
    }
}