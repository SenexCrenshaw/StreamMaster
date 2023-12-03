using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Enums;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using System.Threading;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{
    public async Task<LineupResponse?> GetSubscribedLineups(CancellationToken cancellationToken)
    {
        var ret = await schedulesDirectAPI.GetApiResponse<LineupResponse>(APIMethod.GET, "lineups", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (ret != null)
        {
            logger.LogDebug("Successfully requested listing of subscribed lineups from Schedules Direct.");
        }
        else
        {
            logger.LogError("Did not receive a response from Schedules Direct for list of subscribed lineups.");
        }

        return ret;
    }

    public async Task<Dictionary<string, List<Country>>?> GetAvailableCountries(CancellationToken cancellationToken)
    {
        var ret = await schedulesDirectAPI.GetApiResponse<Dictionary<string, List<Country>>>(APIMethod.GET, "available/countries", cancellationToken: cancellationToken);
        if (ret != null)
        {
            logger.LogDebug("Successfully retrieved list of available countries from Schedules Direct.");
        }
        else
        {
            logger.LogError("Did not receive a response from Schedules Direct for a list of available countries.");
        }
        return ret;
    }

    public async Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default)
    {
        var ret = await schedulesDirectAPI.GetApiResponse<List<Headend>>(APIMethod.GET, $"headends?country={country}&postalcode={postalCode}", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved the headends for {country} and postal code {postalCode}.");
        }
        else
        {
            logger.LogError($"Failed to get a response from Schedules Direct for the headends of {country} and postal code {postalCode}.");
        }

        return ret;
    }

    public async Task<List<LineupPreviewChannel>?> GetLineupPreviewChannel(string lineup)
    {
        List<LineupPreviewChannel>? ret = await schedulesDirectAPI.GetApiResponse<List<LineupPreviewChannel>>(APIMethod.GET, $"lineups/preview/{lineup}");
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved the channels in lineup {lineup} for preview.");
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for retrieval of lineup {lineup} for preview.");
        }

        return ret;
    }

    public async Task<List<LineupPreviewChannel>> GetLineupPreviewChannels(CancellationToken cancellationToken)
    {
        List<LineupPreviewChannel> res = [];
        var lineups = await GetSubscribedLineups(cancellationToken);

        if (lineups is null)
        {
            return [];
        }

        foreach (var lineup in lineups.Lineups)
        {
            List<LineupPreviewChannel>? results = await schedulesDirectAPI.GetApiResponse<List<LineupPreviewChannel>>(APIMethod.GET, $"lineups/preview/{lineup.LineupString}", cancellationToken: cancellationToken).ConfigureAwait(false);

            if (results == null)
            {
                logger.LogError($"Did not receive a response from Schedules Direct for retrieval of lineup {lineup} for preview.");
                continue;
            }

            logger.LogDebug($"Successfully retrieved the channels in lineup {lineup} for preview.");

            for (int index = 0; index < results.Count; index++)
            {
                var lineupPreview = results[index];
                lineupPreview.Channel = lineup.Lineup;
                lineupPreview.Id = index;
                lineupPreview.Affiliate ??= "";
            }

            res.AddRange(results);
        }

      

        return res;
    }
  
    public async Task<bool> AddLineup(string lineup, CancellationToken cancellationToken)
    {
        AddRemoveLineupResponse? ret = await schedulesDirectAPI.GetApiResponse< AddRemoveLineupResponse>(APIMethod.PUT, $"lineups/{lineup}", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (ret != null)
        {
            logger.LogDebug($"Successfully added lineup {lineup} to account. serverID: {ret.ServerId} , message: {ret.Message} , changesRemaining: {ret.ChangesRemaining}");
        }
        else
        {
            logger.LogError($"Failed to get a response from Schedules Direct when trying to add lineup {lineup}.");
        }
        return ret != null;
    }

    public async Task<bool> RemoveLineup(string lineup, CancellationToken cancellationToken)
    {
        AddRemoveLineupResponse? ret = await schedulesDirectAPI.GetApiResponse<AddRemoveLineupResponse>(APIMethod.DELETE, $"lineups/{lineup}", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (ret != null)
        {
            logger.LogDebug($"Successfully removed lineup {lineup} from account. serverID: {ret.ServerId} , message: {ret.Message} , changesRemaining: {ret.ChangesRemaining}");
        }
        else
        {
            logger.LogError($"Failed to get a response from Schedules Direct when trying to remove lineup {lineup}.");
        }
        return ret != null;
    }

    private  async Task<bool> BuildLineupServices(CancellationToken cancellationToken=default)
    {
        var clientLineups = await GetSubscribedLineups(cancellationToken).ConfigureAwait(false);
        if (clientLineups == null || !clientLineups.Lineups.Any()) return false;

        Setting setting = await settingsService.GetSettingsAsync(cancellationToken).ConfigureAwait(false);

        int maxDays = setting.SDEPGDays;
        int maxRatings = setting.SDMaxRatings;
        bool useLineupInName = setting.SDUseLineupInName;


        foreach (var clientLineup in clientLineups.Lineups)
        {
            // don't download station map if lineup not included
            if (setting.SDStationIds.FirstOrDefault(a => a.Lineup == clientLineup.Lineup) == null)
            {
                logger.LogWarning($"Subscribed lineup {clientLineup.Lineup} has been EXCLUDED by user from download and processing.");
                continue;
            }

            if (clientLineup.IsDeleted)
            {
                logger.LogWarning($"Subscribed lineup {clientLineup.Lineup} has been DELETED at the headend.");
                continue;
            }

            // request the lineup's station maps
            var lineupMap = await GetStationChannelMap(clientLineup.Lineup);
            if (lineupMap == null || ((lineupMap?.Stations?.Count ?? 0) == 0))
            {
                logger.LogError($"Subscribed lineup {clientLineup.Lineup} does not contain any stations.");
                return false;
            }

            //HashSet<string> stationsIds = setting.SDStationIds.Select(a => a.StationId).ToHashSet();
            //if (stationsIds.Count == 0)
            //{
            //    logger.LogError($"Subscribed lineup {clientLineup.Lineup} does not contain any stations.");
            //    return false;
            //}

            Dictionary<string, LineupStation> stationDictionary = [];

            List<ChannelLogoDto> channelLogos = memoryCache.ChannelLogos();
            int nextId = channelLogos.Any() ? channelLogos.Max(a => a.Id) + 1 : 0;

            // build the services and lineup
            foreach (var station in lineupMap.Stations)
            {
                // check if station should be downloaded and processed
                if (station == null || stationDictionary.ContainsKey(station.StationId)) continue;

                stationDictionary.Add(station.StationId, station);
            }
        }
        return true;
    }

}