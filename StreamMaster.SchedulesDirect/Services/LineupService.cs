using System.Text.Json;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Caching.Memory;

using SixLabors.ImageSharp;

using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Helpers;
namespace StreamMaster.SchedulesDirect.Services;

public class LineupService(
    ILogger<LineupService> logger,
    ILogger<HybridCacheManager<LineupResult>> cacheLogger,
    IOptionsMonitor<SDSettings> sdSettings,
    ILogoService logoService,
    ISchedulesDirectAPIService schedulesDirectAPI,
    IMemoryCache memoryCache,
    ISchedulesDirectDataService schedulesDirectDataService,
    IImageDownloadQueue imageDownloadQueue
    ) : ILineupService
{
    private readonly HybridCacheManager<LineupResult> hybridCache = new(cacheLogger, memoryCache, useCompression: false, useKeyBasedFiles: true);

    public async Task<bool> BuildLineupServicesAsync(CancellationToken cancellationToken = default)
    {

        LineupResponse? clientLineups = await GetSubscribedLineups(cancellationToken).ConfigureAwait(false);

        if (clientLineups == null || clientLineups.Lineups.Count < 1)
        {
            return true;
        }

        string preferredLogoStyle = string.IsNullOrEmpty(sdSettings.CurrentValue.PreferredLogoStyle) ? "DARK" : sdSettings.CurrentValue.PreferredLogoStyle;
        string alternateLogoStyle = string.IsNullOrEmpty(sdSettings.CurrentValue.AlternateLogoStyle) ? "WHITE" : sdSettings.CurrentValue.AlternateLogoStyle;
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

        foreach (SubscribedLineup clientLineup in clientLineups.Lineups)
        {
            if (clientLineup.IsDeleted)
            {
                if (sdSettings.CurrentValue.SDStationIds.Any(a => a.Lineup == clientLineup.Lineup))
                {
                    sdSettings.CurrentValue.SDStationIds.RemoveAll(a => a.Lineup == clientLineup.Lineup);
                    SettingsHelper.UpdateSetting(sdSettings);
                }
            }

            //CheckHeadendView(clientLineup);

            if (sdSettings.CurrentValue.SDStationIds.Find(a => a.Lineup == clientLineup.Lineup) == null)
            {
                return true;
            }

            //MxfLineup mxfLineup = schedulesDirectData.FindOrCreateLineup(clientLineup.Lineup, $"SM {clientLineup.Name} ({clientLineup.Location})");
            StationChannelMap? lineupMap = await GetStationChannelMap(clientLineup.Lineup);

            if (lineupMap == null || lineupMap.Stations == null || lineupMap.Stations.Count == 0)
            {
                logger.LogError("Subscribed lineup {clientLineup.Lineup} does not contain any stations.", clientLineup.Lineup);
                continue;
            }

            ConcurrentHashSet<string> channelNumbers = [];

            foreach (LineupStation? station in lineupMap.Stations)
            {
                if (station == null || sdSettings.CurrentValue.SDStationIds.Find(a => a.StationId == station.StationId) == null)
                {
                    continue;
                }

                MxfService mxfService = schedulesDirectData.FindOrCreateService(station.StationId);

                if (string.IsNullOrEmpty(mxfService.CallSign))
                {
                    SetStationDetails(station, mxfService, preferredLogoStyle, alternateLogoStyle);
                }
            }
        }

        if (!schedulesDirectData.Services.IsEmpty)
        {
            //UpdateIcons(schedulesDirectData.Services.Values);
            logger.LogInformation("Exiting BuildLineupServices(). SUCCESS.");
            //  epgCache.SaveCache();
            return true;
        }

        logger.LogWarning("There are 0 stations queued for download.");
        return false;
    }

    private void SetStationDetails(LineupStation station, MxfService mxfService, string preferredLogoStyle, string alternateLogoStyle)
    {
        if (!string.IsNullOrEmpty(mxfService.CallSign))
        {
            return; // If the callsign is already set, there's no need to continue
        }

        // Set the UID and CallSign for the service
        //mxfService.UidOverride = $"!Service!STREAMMASTER_{station.StationId}";
        mxfService.CallSign = station.Callsign;

        // Set the service name based on the station name and callsign
        if (string.IsNullOrEmpty(mxfService.Name))
        {
            mxfService.Name = Regex.Matches(station.Name.Replace("-", ""), station.Callsign).Count > 0
                ? !string.IsNullOrEmpty(station.Affiliate) ? $"{station.Name} ({station.Affiliate})" : station.Name
                : station.Name;
        }

        // Add the affiliate if it exists
        if (!string.IsNullOrEmpty(station.Affiliate))
        {
            mxfService.mxfAffiliate = schedulesDirectDataService.SchedulesDirectData().FindOrCreateAffiliate(station.Affiliate);
        }

        // Handle station logo if available
        StationImage? stationLogo = GetStationLogo(station, preferredLogoStyle, alternateLogoStyle);
        if (stationLogo != null)
        {
            mxfService.XmltvIcon = new XmltvIcon
            {
                Src = stationLogo.Url,
                Width = stationLogo.Width,
                Height = stationLogo.Height
            };

            //schedulesDirectDataService.SchedulesDirectData().FindOrCreateProgramArtwork(stationLogo.Url);
            string title = string.IsNullOrEmpty(station.Callsign) ? station.Name : $"{station.Callsign} {station.Name}";

            LogoInfo logoInfo = new(title, stationLogo.Url, SMFileTypes.Logo, false);

            CustomLogoDto d = new() { Source = logoInfo.FileName, Value = stationLogo.Url, Name = title };
            logoService.CacheLogo(d);
            imageDownloadQueue.EnqueueLogo(logoInfo);
        }
    }
    private static StationImage? GetStationLogo(LineupStation station, string preferredLogoStyle, string alternateLogoStyle)
    {
        // Select the logo based on preferred or alternate styles, falling back to the default logo
        return station.StationLogos?.FirstOrDefault(arg => arg.Category?.EqualsIgnoreCase(preferredLogoStyle) == true)
               ?? station.StationLogos?.FirstOrDefault(arg => arg.Category?.EqualsIgnoreCase(alternateLogoStyle) == true)
               ?? station.Logo;
    }

    private async Task<LineupResponse?> GetSubscribedLineups(CancellationToken cancellationToken)
    {
        LineupResponse? response = await schedulesDirectAPI.GetApiResponse<LineupResponse>(APIMethod.GET, "lineups", cancellationToken, cancellationToken).ConfigureAwait(false);
        return response;
    }

    private async Task<StationChannelMap?> GetStationChannelMap(string lineup)
    {
        return await schedulesDirectAPI.GetApiResponse<StationChannelMap>(APIMethod.GET, $"lineups/{lineup}").ConfigureAwait(false);
    }

    public async Task<List<SubscribedLineup>> GetLineups(CancellationToken cancellationToken = default)
    {
        if (!sdSettings.CurrentValue.SDEnabled)
        {
            return [];
        }

        LineupResponse? lineups = await GetSubscribedLineups(cancellationToken).ConfigureAwait(false);
        return lineups?.Lineups ?? [];
    }

    public async Task<List<StationChannelMap>> GetStationChannelMaps(CancellationToken cancellationToken)
    {
        List<StationChannelMap> result = [];
        List<Station> stations = await GetStations(cancellationToken).ConfigureAwait(false);

        foreach (Station station in stations)
        {
            StationChannelMap? stationChannelMap = await GetStationChannelMap(station.Lineup).ConfigureAwait(false);
            if (stationChannelMap != null)
            {
                result.Add(stationChannelMap);
            }
        }

        return result;
    }

    public async Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken)
    {
        List<Station> stations = await GetStations(cancellationToken).ConfigureAwait(false);
        if (stations == null || stations.Count == 0)
        {
            return [];
        }

        List<StationPreview> previews = [];
        foreach (Station station in stations)
        {
            StationPreview preview = new(station)
            {
                Affiliate = station.Affiliate ?? string.Empty
            };
            previews.Add(preview);
        }

        return previews;
    }

    private async Task<List<Station>> GetStations(CancellationToken cancellationToken)
    {
        List<Station> stations = [];
        List<SubscribedLineup> lineups = await GetLineups(cancellationToken).ConfigureAwait(false);

        foreach (SubscribedLineup lineup in lineups)
        {
            LineupResult? lineupResult = await GetLineup(lineup.Lineup, cancellationToken).ConfigureAwait(false);
            if (lineupResult == null)
            {
                continue;
            }

            stations.AddRange(lineupResult.Stations.Select(station =>
            {
                station.Lineup = lineup.Lineup;
                return station;
            }));
        }

        return stations;
    }

    private async Task<LineupResult?> GetLineup(string lineup, CancellationToken cancellationToken)
    {
        string? cachedData = await hybridCache.GetAsync(lineup);

        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                // Deserialize the cached data to LineupResult
                LineupResult? cachedLineup = JsonSerializer.Deserialize<LineupResult>(cachedData);
                if (cachedLineup != null)
                {
                    return cachedLineup;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while deserializing cached lineup data for {LineupKey}", lineup);
            }
        }

        // Fetch lineup data if not found in cache
        LineupResult? lineupResult = await schedulesDirectAPI
            .GetApiResponse<LineupResult>(APIMethod.GET, $"lineups/{lineup}", cancellationToken, cancellationToken)
            .ConfigureAwait(false);

        if (lineupResult != null)
        {
            try
            {
                // Serialize the lineup result to JSON and save it in the cache
                string serializedData = JsonSerializer.Serialize(lineupResult);
                await hybridCache.SetAsync(lineup, serializedData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while serializing and caching lineup data for {LineupKey}", lineup);
            }
        }

        return lineupResult;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public List<string> GetExpiredKeys()
    {
        return hybridCache.GetExpiredKeysAsync().Result;
    }

    public void RemovedExpiredKeys(List<string>? keysToDelete = null) { }
}