using StreamMaster.Application.Services;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Helpers;
using StreamMaster.SchedulesDirect;

namespace StreamMaster.Application.Settings.Commands;

public class UpdateSettingRequest : IRequest<UpdateSettingResponse>
{
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
    public bool? M3UFieldGroupTitle { get; set; }
    public bool? M3UStationId { get; set; }
    public bool? M3UUseChnoForId { get; set; }
    public bool? M3UIgnoreEmptyEPGID { get; set; }
    public bool? M3UUseCUIDForChannelID { get; set; }
    public bool? PrettyEPG { get; set; }
    public int? MaxConnectRetry { get; set; }
    public int? MaxConnectRetryTimeMS { get; set; }
    public string? SSLCertPassword { get; set; }
    public string? SSLCertPath { get; set; }
    public string? StreamingClientUserAgent { get; set; }
    public StreamingProxyTypes? StreamingProxyType { get; set; }
    public bool? VideoStreamAlwaysUseEPGLogo { get; set; }
    public bool? EnablePrometheus { get; set; }
    public int? MaxLogFiles { get; set; }
    public int? MaxLogFileSizeMB { get; set; }
    public List<string>? NameRegex { get; set; } = [];
}

public partial class UpdateSettingRequestHandler(IBackgroundTaskQueue taskQueue, IOptionsMonitor<SDSettings> intsdsettings, ILogger<UpdateSettingRequest> Logger, IMapper Mapper, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext, IOptionsMonitor<Setting> intsettings)
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
        Setting currentSetting = settings;

        bool needsLogOut = await UpdateSetting(currentSetting, sdsettings, request, cancellationToken);

        Logger.LogInformation("UpdateSettingRequest");
        SettingsHelper.UpdateSetting(currentSetting);

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
    private async Task<bool> UpdateSetting(Setting currentSetting, SDSettings sdsettings, UpdateSettingRequest request, CancellationToken cancellationToken)
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
            CopyNonNullFields(request.SDSettings, sdsettings);
            SettingsHelper.UpdateSetting(sdsettings);
        }

        if (request.EnableSSL != null && request.EnableSSL != currentSetting.EnableSSL)
        {
            currentSetting.EnableSSL = (bool)request.EnableSSL;
        }

        if (request.VideoStreamAlwaysUseEPGLogo != null && request.VideoStreamAlwaysUseEPGLogo != currentSetting.VideoStreamAlwaysUseEPGLogo)
        {
            currentSetting.VideoStreamAlwaysUseEPGLogo = (bool)request.VideoStreamAlwaysUseEPGLogo;
        }

        if (request.PrettyEPG.HasValue)
        {
            currentSetting.PrettyEPG = request.PrettyEPG.Value;
        }

        if (request.M3UIgnoreEmptyEPGID != null)
        {
            currentSetting.M3UIgnoreEmptyEPGID = (bool)request.M3UIgnoreEmptyEPGID;
        }

        if (request.M3UUseCUIDForChannelID != null)
        {
            currentSetting.M3UUseCUIDForChannelID = (bool)request.M3UUseCUIDForChannelID;
        }

        if (request.MaxLogFiles != null)
        {
            currentSetting.MaxLogFiles = (int)request.MaxLogFiles;
        }

        if (request.MaxLogFileSizeMB != null)
        {
            currentSetting.MaxLogFileSizeMB = (int)request.MaxLogFileSizeMB;
        }

        if (request.EnablePrometheus != null)
        {
            currentSetting.EnablePrometheus = (bool)request.EnablePrometheus;
        }

        if (request.MaxLogFiles != null)
        {
            currentSetting.MaxLogFiles = (int)request.MaxLogFiles;
        }
        if (request.M3UUseChnoForId != null)
        {
            currentSetting.M3UUseChnoForId = (bool)request.M3UUseChnoForId;
        }

        if (request.BackupEnabled != null)
        {
            currentSetting.BackupEnabled = (bool)request.BackupEnabled;
        }

        if (request.BackupVersionsToKeep.HasValue)
        {
            currentSetting.BackupVersionsToKeep = request.BackupVersionsToKeep.Value;
        }

        if (request.BackupInterval.HasValue)
        {
            currentSetting.BackupInterval = request.BackupInterval.Value;
        }

        if (request.ShowClientHostNames != null)
        {
            currentSetting.ShowClientHostNames = (bool)request.ShowClientHostNames;
        }

        if (request.DummyRegex != null)
        {
            currentSetting.DummyRegex = request.DummyRegex;
        }

        if (request.M3UStationId != null)
        {
            currentSetting.M3UStationId = (bool)request.M3UStationId;
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
}