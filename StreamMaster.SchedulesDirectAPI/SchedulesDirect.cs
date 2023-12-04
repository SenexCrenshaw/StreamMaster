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
    private readonly object fileLock = new();
    private const int MaxQueries = 1250;
    private const int MaxImgQueries = 125;
    private const int MaxParallelDownloads = 4;

    private static int processedObjects;
    private static int totalObjects;
    private static int processStage;
    private readonly ILogger _logger = logger;
  

    //public async Task<bool> SDSync( CancellationToken cancellationToken)
    //{
    //    var lineups = await GetSubscribedLineups(cancellationToken).ConfigureAwait(false);
    //    return await SDSync([], cancellationToken).ConfigureAwait(false);
    //}
    public async Task<bool> SDSync(CancellationToken cancellationToken)
    {      
        if (!await EnsureToken(cancellationToken).ConfigureAwait(false))
        {
            _logger.LogWarning("Schedules Direct Token Not Ready");
            return false;
        }
        //UserStatus status = await GetStatus(cancellationToken);

        if (!await GetSystemReady(cancellationToken))
        {
            _logger.LogWarning("Schedules Direct Not Ready");
            return false;
        }

        var startTime = DateTime.UtcNow;
        var setting = memoryCache.GetSetting();
        logger.LogInformation($"DaysToDownload: {setting.SDEPGDays}");


        //DateTime now = DateTime.Now;

        //List<Station> stations = await GetStations(cancellationToken).ConfigureAwait(false);
        //List<Schedule>? schedules = await GetSchedules(StationIdLineups.ConvertAll(a => a.StationId), cancellationToken).ConfigureAwait(false);
        //List<string> progIds = schedules.SelectMany(a => a.Programs).Where(a => a.AirDateTime >= now.AddDays(-1) && a.AirDateTime <= now.AddDays(setting.SDEPGDays)).Select(a => a.ProgramID).Distinct().ToList();
        //List<SDProgram> programs = await GetSDPrograms(progIds, cancellationToken).ConfigureAwait(false);

        // load cache file
        epgCache.LoadCache();
        if (
             await BuildLineupServices(cancellationToken) &&
             await GetAllScheduleEntryMd5S(setting.SDEPGDays) &&
              BuildAllProgramEntries()
            )
        {
            epgCache.WriteCache();
            //do good things
            return true;
        }

        return false;
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