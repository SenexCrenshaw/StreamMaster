using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain;
using StreamMaster.SchedulesDirectAPI.Domain.Commands;
using StreamMaster.SchedulesDirectAPI.Domain.Enums;
using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterDomain.Common;
using StreamMasterDomain.Services;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect(ILogger<SchedulesDirect> logger,IEPGCache epgCache, ISchedulesDirectData schedulesDirectData, ISchedulesDirectAPI schedulesDirectAPI, ISettingsService settingsService, ISDToken SdToken, IMemoryCache memoryCache) : ISchedulesDirect
{
    public bool IsSyncing { get; set; }

    private readonly object fileLock = new();
    private const int MaxQueries = 1250;
    private const int MaxImgQueries = 125;
    private const int MaxParallelDownloads = 4;

    private static int processedObjects;
    private static int totalObjects;
    private static int processStage;
    private readonly ILogger _logger = logger;

    public async Task<bool> SDSync(CancellationToken cancellationToken)
    {
        if (IsSyncing)
        {
            _logger.LogWarning("Schedules Direct already Syncing");
            return false;
        }

        IsSyncing = true;
        if (!await EnsureToken(cancellationToken).ConfigureAwait(false))
        {
            _logger.LogWarning("Schedules Direct Token Not Ready");
            IsSyncing = false;
            return false;
        }
        //UserStatus status = await GetStatus(cancellationToken);

        if (!await GetSystemReady(cancellationToken))
        {
            _logger.LogWarning("Schedules Direct Not Ready");
            IsSyncing = false;
            return false;
        }

        //var startTime = DateTime.UtcNow;
        var setting = memoryCache.GetSetting();
        logger.LogInformation($"DaysToDownload: {setting.SDSettings.SDEPGDays}");

        // load cache file
        epgCache.LoadCache();
        if (
             await BuildLineupServices(cancellationToken) &&
                await GetAllScheduleEntryMd5S(setting.SDSettings.SDEPGDays) &&
                BuildAllProgramEntries() &&
                BuildAllGenericSeriesInfoDescriptions() &&
                await GetAllMoviePosters(cancellationToken) &&
                await GetAllSeriesImages(cancellationToken) &&
                GetAllSeasonImages() &&
                GetAllSportsImages() &&
                BuildKeywords()
            )
        {
            //epgCache.WriteCache();
            CreateDummLineupChannel();
            var xml = CreateXmltv("");
            if (xml is not null)
            {
                WriteXmltv(xml);
            }
            
            logger.LogInformation("Completed Schedules Direct update execution. SUCCESS.");
            IsSyncing = false;
            return true;
        }
        //StationLogosToDownload = [];
        IsSyncing = false;
        return false;
    }

    private  void WriteXmltv(XMLTV xmltv)
    {
        var fileName = Path.Combine(BuildInfo.SDCacheFolder, "epg123.xmltv");
        if (!FileUtil.WriteXmlFile(xmltv, fileName)) return;

        var fi = new FileInfo(fileName);
        var imageCount = xmltv.Programs.SelectMany(program => program.Icons?.Select(icon => icon.Src) ?? new List<string>()).Distinct().Count();
        logger.LogInformation($"Completed save of the XMLTV file to \"{fileName}\". ({FileUtil.BytesToString(fi.Length)})");
        logger.LogDebug($"Generated XMLTV file contains {xmltv.Channels.Count} channels and {xmltv.Programs.Count} programs with {imageCount} distinct program image links.");
    }

    private  void CreateDummLineupChannel()
    {
        var mxfService = schedulesDirectData.FindOrCreateService("DUMMY");
        mxfService.CallSign = "DUMMY";
        mxfService.Name = "DUMMY Station";

        var mxfLineup = schedulesDirectData.FindOrCreateLineup("ZZZ-DUMMY-EPG123", "ZZZ123 Dummy Lineup");
        mxfLineup.channels.Add(new MxfChannel(mxfLineup, mxfService));
    }

    //private  bool ServiceCountSafetyCheck()
    //{
    //    var setting = memoryCache.GetSetting();

    //    if (setting.ExpectedServiceCount < 20 || !(setting.SDStationIds.Count < setting.ExpectedServiceCount * 0.95)) return true;
    //    logger.LogError($"Of the expected {setting.SDStationIds.Count} stations to download, there are only {config.ExpectedServicecount - MissingStations} stations available from Schedules Direct. Aborting update for review by user.");
    //    return false;
    //}

    public async Task<bool> GetSystemReady(CancellationToken cancellationToken)
    {
        UserStatus status = await GetStatusInternal(cancellationToken);

        try
        {
            return status.SystemStatus.Any() && status.SystemStatus[0].Status?.ToLower() == "online";
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> EnsureToken(CancellationToken cancellationToken)
    {
        return await SdToken.GetTokenAsync(cancellationToken) != null;
    }
    public async Task<UserStatus> GetStatus(CancellationToken cancellationToken)
    {
        UserStatus status = await GetStatusInternal(cancellationToken);
        return status;
    }
    public void ResetCache(string command)
    {
        string cacheKey = SDHelpers.GenerateCacheKey(command);
        string cachePath = Path.Combine(BuildInfo.SDCacheFolder, cacheKey);

        if (File.Exists(cachePath))
        {
            File.Delete(cachePath);
        }
    }

    private async Task<UserStatus> GetStatusInternal(CancellationToken cancellationToken)
    {
        UserStatus? result = await schedulesDirectAPI.GetApiResponse<UserStatus>(APIMethod.GET, SDCommands.Status, cancellationToken).ConfigureAwait(false);
        if (result == null)
        {
            return SDHelpers.GetStatusOffline();
        }
        result = await HandleStatus(result, cancellationToken).ConfigureAwait(false);

        return result ?? SDHelpers.GetStatusOffline();
    }

    private async Task<UserStatus?> HandleStatus(UserStatus UserStatus, CancellationToken cancellationToken)
    {
        if ((SDHttpResponseCode)UserStatus.Code is SDHttpResponseCode.ACCOUNT_LOCKOUT or SDHttpResponseCode.ACCOUNT_DISABLED or SDHttpResponseCode.ACCOUNT_EXPIRED or SDHttpResponseCode.TOKEN_EXPIRED or SDHttpResponseCode.INVALID_USER)
        {
            if (await SdToken.ResetTokenAsync(cancellationToken).ConfigureAwait(false) == null)
            {
                return null;
            }
            ResetCache(SDCommands.Status);
            return await GetStatusInternal(cancellationToken).ConfigureAwait(false);
        }
        return UserStatus;
    }
}