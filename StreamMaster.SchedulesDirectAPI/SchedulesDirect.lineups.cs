using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Enums;


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

    public async Task<List<CountryData>?> GetAvailableCountries(CancellationToken cancellationToken)
    {
        var ret = await schedulesDirectAPI.GetApiResponse<Dictionary<string, List<Country>>>(APIMethod.GET, "available/countries", cancellationToken: cancellationToken);
        if (ret == null)  
        {
            logger.LogError("Did not receive a response from Schedules Direct for a list of available countries.");
            return null;
        }

        logger.LogDebug("Successfully retrieved list of available countries from Schedules Direct.");
        List<CountryData> serializableDataList = ret
         .Select(kv => new CountryData { Key = kv.Key, Countries = kv.Value })
         .ToList();

        return serializableDataList;
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

    public async Task<LineupResult?> GetLineup(string lineup, CancellationToken cancellationToken)
    {
        return await schedulesDirectAPI.GetApiResponse<LineupResult>(APIMethod.GET, $"lineups/{lineup}", cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<SubscribedLineup>> GetLineups(CancellationToken cancellationToken)
    {
        List<LineupPreviewChannel> res = [];
        var lineups = await GetSubscribedLineups(cancellationToken);

        if (lineups is null)
        {
            return [];
        }

        return lineups.Lineups;
    }

    public async Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken)
    {
        List<Station>? stations = await GetStations(cancellationToken).ConfigureAwait(false);
        if (stations is null)
        {
            return [];
        }
        List<StationPreview> ret = [];
        for (int index = 0; index < stations.Count; index++)
        {
            Station station = stations[index];
            StationPreview sp = new(station);
            sp.Affiliate ??= "";
            ret.Add(sp);
        }
        return ret;
    }

  

    public async Task<List<Station>> GetStations(CancellationToken cancellationToken)
    {
        List<Station> ret = [];

        List<SubscribedLineup> lineups = await GetLineups(cancellationToken).ConfigureAwait(false);
        if (lineups?.Any() != true)
        {
            return ret;
        }

        foreach (SubscribedLineup lineup in lineups)
        {
            LineupResult? res = await GetLineup(lineup.Lineup, cancellationToken).ConfigureAwait(false);
            if (res == null)
            {
                continue;
            }

            foreach (Station station in res.Stations)
            {
                station.Lineup = lineup.Lineup;
            }

            HashSet<string> existingIds = new(ret.Select(station => station.StationId));

            foreach (Station station in res.Stations)
            {
                station.Lineup = lineup.Lineup;
                if (!existingIds.Contains(station.StationId))
                {
                    ret.Add(station);
                    _ = existingIds.Add(station.StationId);
                }
            }
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
            List<LineupPreviewChannel>? results = await schedulesDirectAPI.GetApiResponse<List<LineupPreviewChannel>>(APIMethod.GET, $"lineups/preview/{lineup.Lineup}", cancellationToken: cancellationToken).ConfigureAwait(false);

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

    
}