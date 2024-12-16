using System.Text.Json;
using System.Text.RegularExpressions;

using SixLabors.ImageSharp;

using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Helpers;

namespace StreamMaster.SchedulesDirect.Services;

public class LineupService(
    ILogger<LineupService> logger,
    IOptionsMonitor<SDSettings> sdSettings,
    ILogoService logoService,
    ISchedulesDirectAPIService schedulesDirectAPI,
    SMCacheManager<LineupResult> LineupResultCache,
    ISchedulesDirectDataService schedulesDirectDataService,
    IImageDownloadQueue imageDownloadQueue
    ) : ILineupService
{
    public async Task<bool> BuildLineupServicesAsync(CancellationToken cancellationToken = default)
    {
        LineupResponse? clientLineups = await schedulesDirectAPI.GetSubscribedLineupsAsync(cancellationToken).ConfigureAwait(false);

        if (clientLineups == null || clientLineups.Lineups.Count == 0)
        {
            return true;
        }

        string preferredLogoStyle = string.IsNullOrEmpty(sdSettings.CurrentValue.PreferredLogoStyle) ? "DARK" : sdSettings.CurrentValue.PreferredLogoStyle;
        string alternateLogoStyle = string.IsNullOrEmpty(sdSettings.CurrentValue.AlternateLogoStyle) ? "WHITE" : sdSettings.CurrentValue.AlternateLogoStyle;
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;

        await Parallel.ForEachAsync(clientLineups.Lineups, cancellationToken, async (clientLineup, ct) =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (clientLineup.IsDeleted)
            {
                // Remove deleted lineups from settings
                if (sdSettings.CurrentValue.SDStationIds.Any(a => a.Lineup == clientLineup.Lineup))
                {
                    sdSettings.CurrentValue.SDStationIds.RemoveAll(a => a.Lineup == clientLineup.Lineup);
                    SettingsHelper.UpdateSetting(sdSettings);
                }
            }

            // Skip processing if the lineup already exists
            if (sdSettings.CurrentValue.SDStationIds.Find(a => a.Lineup == clientLineup.Lineup) == null)
            {
                return; // Equivalent of `continue` in this context
            }

            // Fetch lineup details asynchronously
            LineupResult? lineupMap = await GetLineupAsync(clientLineup.Lineup, ct);

            if (lineupMap == null || lineupMap.Stations == null || lineupMap.Stations.Count == 0)
            {
                logger.LogError("Subscribed lineup {clientLineup.Lineup} does not contain any stations.", clientLineup.Lineup);
                return;
            }

            // Use a thread-safe collection for channel numbers
            ConcurrentHashSet<string> channelNumbers = [];

            // Process stations in parallel
            await Parallel.ForEachAsync(lineupMap.Stations, ct, async (station, _) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (station == null || station.StationId == null || sdSettings.CurrentValue.SDStationIds.Find(a => a.StationId == station.StationId) == null)
                {
                    return;
                }

                // Find or create the MxfService for the station
                MxfService mxfService = schedulesDirectData.FindOrCreateService(station.StationId);

                // Populate station details if necessary
                if (string.IsNullOrEmpty(mxfService.CallSign))
                {
                    SetStationDetails(station, mxfService, preferredLogoStyle, alternateLogoStyle);
                }
            });
        });

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

    private void SetStationDetails(Station station, MxfService mxfService, string preferredLogoStyle, string alternateLogoStyle)
    {
        if (!string.IsNullOrEmpty(mxfService.CallSign) || station.Name is null || station.Callsign is null)
        {
            return;
        }

        mxfService.CallSign = station.Callsign;

        // Set the service name based on the station name and callsign
        if (string.IsNullOrEmpty(mxfService.Name))
        {
            mxfService.Name = Regex.Matches(station.Name.Replace("-", ""), station.Callsign).Count > 0
                ? !string.IsNullOrEmpty(station.Affiliate) ? $"{station.Name} ({station.Affiliate})" : station.Name
                : station.Name;
        }

        // Handle station logo if available
        Logo? stationLogo = GetStationLogo(station, preferredLogoStyle, alternateLogoStyle);
        if (stationLogo != null)
        {
            mxfService.XmltvIcon = new XmltvIcon
            {
                Src = stationLogo.Url,
                Width = stationLogo.Width,
                Height = stationLogo.Height
            };

            //schedulesDirectDataService.SchedulesDirectData.FindOrCreateProgramArtwork(stationLogo.Url);
            string? title = string.IsNullOrEmpty(station.Callsign) ? station.Name : $"{station.Callsign} {station.Name}";

            LogoInfo logoInfo = new(title, stationLogo.Url, SMFileTypes.Logo, false);

            CustomLogoDto d = new() { Source = logoInfo.FileName, Value = stationLogo.Url, Name = title };
            logoService.CacheLogo(d);
            imageDownloadQueue.EnqueueLogo(logoInfo);
        }
    }

    private static Logo? GetStationLogo(Station station, string preferredLogoStyle, string alternateLogoStyle)
    {
        // Select the logo based on preferred or alternate styles, falling back to the default logo
        return station.StationLogos?.FirstOrDefault(arg => arg.Category?.EqualsIgnoreCase(preferredLogoStyle) == true)
               ?? station.StationLogos?.FirstOrDefault(arg => arg.Category?.EqualsIgnoreCase(alternateLogoStyle) == true)
               ?? station.Logo;
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
            cancellationToken.ThrowIfCancellationRequested();
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
        LineupResponse? lineups = await schedulesDirectAPI.GetSubscribedLineupsAsync(cancellationToken).ConfigureAwait(false);
        if (lineups == null || lineups.Lineups.Count == 0)
        {
            return stations;
        }

        foreach (SubscribedLineup lineup in lineups.Lineups)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LineupResult? lineupResult = await GetLineupAsync(lineup.Lineup, cancellationToken).ConfigureAwait(false);
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

    private async Task<LineupResult?> GetLineupAsync(string lineup, CancellationToken cancellationToken)
    {
        try
        {
            LineupResult? cachedLineup = await LineupResultCache.GetAsync<LineupResult>(lineup);
            if (cachedLineup != null)
            {
                return cachedLineup;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while deserializing cached lineup data for {LineupKey}", lineup);
        }

        LineupResult? lineupResult = await schedulesDirectAPI.GetLineupResultAsync(lineup, cancellationToken).ConfigureAwait(false);

        if (lineupResult != null)
        {
            try
            {
                string serializedData = JsonSerializer.Serialize(lineupResult);
                await LineupResultCache.SetAsync(lineup, serializedData);
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
        return LineupResultCache.GetExpiredKeysAsync().Result;
    }

    public void RemovedExpiredKeys(List<string>? keysToDelete = null)
    { }
}