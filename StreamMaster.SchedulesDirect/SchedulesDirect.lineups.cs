using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Cache;
using StreamMaster.SchedulesDirect.Domain.Enums;


namespace StreamMaster.SchedulesDirect;
public partial class SchedulesDirect
{
    public async Task<LineupResponse?> GetSubscribedLineups(CancellationToken cancellationToken)
    {
        //LineupResponse? cache = await GetValidCachedDataAsync<LineupResponse>("SubscribedLineups", cancellationToken).ConfigureAwait(false);
        //if (cache != null)
        //{
        //    return cache;
        //}


        LineupResponse? cache = await schedulesDirectAPI.GetApiResponse<LineupResponse>(APIMethod.GET, "lineups", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (cache != null)
        {
            logger.LogDebug("Successfully requested listing of subscribed lineups from Schedules Direct.");
            //await WriteToCacheAsync("SubscribedLineups", cache, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            logger.LogError("Did not receive a response from Schedules Direct for list of subscribed lineups.");
        }

        return cache;
    }

    public async Task<List<CountryData>?> GetAvailableCountries(CancellationToken cancellationToken)
    {
        List<CountryData>? cache = await GetValidCachedDataAsync<List<CountryData>>("AvailableCountries", cancellationToken).ConfigureAwait(false);
        if (cache != null)
        {
            return cache;
        }

        Dictionary<string, List<Country>>? ret = await schedulesDirectAPI.GetApiResponse<Dictionary<string, List<Country>>>(APIMethod.GET, "available/countries", cancellationToken: cancellationToken);
        if (ret == null)
        {
            logger.LogError("Did not receive a response from Schedules Direct for a list of available countries.");
            return null;
        }

        logger.LogDebug("Successfully retrieved list of available countries from Schedules Direct.");
        List<CountryData> serializableDataList = ret
         .Select(kv => new CountryData { Key = kv.Key, Countries = kv.Value })
         .ToList();

        await WriteToCacheAsync("AvailableCountries", serializableDataList, cancellationToken).ConfigureAwait(false);

        return serializableDataList;
    }

    public async Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(country) || string.IsNullOrEmpty(postalCode))
        {
            logger.LogWarning($"Country {country} or postal code {postalCode} is empty");
            return null;
        }

        List<Headend>? cache = await GetValidCachedDataAsync<List<Headend>>($"Headends-{country}-{postalCode}", cancellationToken).ConfigureAwait(false);
        if (cache != null)
        {
            return cache;
        }

        cache = await schedulesDirectAPI.GetApiResponse<List<Headend>>(APIMethod.GET, $"headends?country={country}&postalcode={postalCode}", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (cache != null)
        {
            await WriteToCacheAsync($"Headends-{country}-{postalCode}", cache, cancellationToken).ConfigureAwait(false);
            logger.LogDebug($"Successfully retrieved the headends for {country} and postal code {postalCode}.");
        }
        else
        {
            logger.LogError($"Failed to get a response from Schedules Direct for the headends of {country} and postal code {postalCode}.");
        }

        return cache;
    }

    public async Task<List<LineupPreviewChannel>?> GetLineupPreviewChannel(string lineup, CancellationToken cancellationToken)
    {
        List<LineupPreviewChannel>? ret = await GetValidCachedDataAsync<List<LineupPreviewChannel>>("LineupPreviewChannel" + lineup, cancellationToken).ConfigureAwait(false);

        if (ret != null)
        {
            return ret;
        }

        ret = await schedulesDirectAPI.GetApiResponse<List<LineupPreviewChannel>>(APIMethod.GET, $"lineups/preview/{lineup}", cancellationToken: cancellationToken);
        if (ret != null)
        {
            for (int i = 0; i < ret.Count; i++)
            {
                ret[i].Id = i;
            }

            await WriteToCacheAsync("LineupPreviewChannel" + lineup, ret, cancellationToken).ConfigureAwait(false);
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
        LineupResult? cache = await GetValidCachedDataAsync<LineupResult>("Lineup-" + lineup, cancellationToken).ConfigureAwait(false);
        if (cache != null)
        {
            return cache;
        }
        cache = await schedulesDirectAPI.GetApiResponse<LineupResult>(APIMethod.GET, $"lineups/{lineup}", cancellationToken, cancellationToken).ConfigureAwait(false);
        if (cache != null)
        {
            await WriteToCacheAsync("Lineup-" + lineup, cache, cancellationToken).ConfigureAwait(false);
            logger.LogDebug($"Successfully retrieved the channels in lineup {lineup}.");
            return cache;
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for retrieval of lineup {lineup}.");
        }
        return null;
    }

    public async Task<List<SubscribedLineup>> GetLineups(CancellationToken cancellationToken)
    {
        List<LineupPreviewChannel> res = [];
        LineupResponse? lineups = await GetSubscribedLineups(cancellationToken);

        return lineups is null ? ([]) : lineups.Lineups;
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

    //public async Task<List<LineupPreviewChannel>> GetLineupPreviewChannels(CancellationToken cancellationToken)
    //{
    //    List<LineupPreviewChannel> res = [];
    //    var lineups = await GetSubscribedLineups(cancellationToken);

    //    if (lineups is null)
    //    {
    //        return [];
    //    }

    //    foreach (var lineup in lineups.Lineups)
    //    {
    //        List<LineupPreviewChannel>? results = await schedulesDirectAPI.GetApiResponse<List<LineupPreviewChannel>>(APIMethod.GET, $"lineups/preview/{lineup.Lineup}", cancellationToken: cancellationToken).ConfigureAwait(false);

    //        if (results == null)
    //        {
    //            logger.LogError($"Did not receive a response from Schedules Direct for retrieval of lineup {lineup} for preview.");
    //            continue;
    //        }

    //        logger.LogDebug($"Successfully retrieved the channels in lineup {lineup} for preview.");

    //        for (int index = 0; index < results.Count; index++)
    //        {
    //            var lineupPreview = results[index];
    //            lineupPreview.Channel = lineup.Lineup;
    //            lineupPreview.Id = index;
    //            lineupPreview.Affiliate ??= "";
    //        }

    //        res.AddRange(results);
    //    }



    //    return res;
    //}

    public async Task<bool> AddLineup(string lineup, CancellationToken cancellationToken)
    {
        AddRemoveLineupResponse? ret = await schedulesDirectAPI.GetApiResponse<AddRemoveLineupResponse>(APIMethod.PUT, $"lineups/{lineup}", cancellationToken: cancellationToken).ConfigureAwait(false);
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
            memoryCache.SetSyncForceNextRun(true);
        }
        else
        {
            logger.LogError("Failed to get a response from Schedules Direct when trying to remove lineup {lineup}.", lineup);
        }
        return ret != null;
    }


}