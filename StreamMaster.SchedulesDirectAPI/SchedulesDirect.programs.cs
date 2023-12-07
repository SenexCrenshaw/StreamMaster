using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Enums;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{

    public async Task<List<StationChannelMap>> GetStationChannelMaps(CancellationToken cancellationToken)
    {

        var ret = new List<StationChannelMap>();
        foreach (var station in await GetStations(cancellationToken))
        {
            var s = await GetStationChannelMap(station.Lineup);
            if (s is not null)
            {
                ret.Add(s);
            }
        }
        return ret;
    }

    public async Task<StationChannelMap?> GetStationChannelMap(string lineup)
    {
        StationChannelMap? cache = await GetValidCachedDataAsync<StationChannelMap>("StationChannelMap-" + lineup).ConfigureAwait(false);
        if (cache != null)
        {
            return cache;
        }


        StationChannelMap? ret = await schedulesDirectAPI.GetApiResponse<StationChannelMap>(APIMethod.GET, $"lineups/{lineup}");
        if (ret != null)
        {
            await WriteToCacheAsync("StationChannelMap-" + lineup, ret).ConfigureAwait(false);

            logger.LogDebug($"Successfully retrieved the station mapping for lineup {lineup}. ({ret.Stations.Count} stations; {ret.Map.Count} channels)");
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for retrieval of lineup {lineup}.");
        }

        return ret;
    }

    public async Task<List<Programme>?> GetProgramsAsync(string[] request)
    {
        DateTime dtStart = DateTime.Now;
        List<Programme>? ret = await schedulesDirectAPI.GetApiResponse<List<Programme>?>(APIMethod.POST, "programs", request);
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved {ret.Count}/{request.Length} program descriptions. ({DateTime.Now - dtStart:G})");
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for {request.Length} program descriptions. ({DateTime.Now - dtStart:G})");
        }

        return ret;
    }

    public async Task<Dictionary<string, GenericDescription>?> GetGenericDescriptionsAsync(string[] request)
    {
        DateTime dtStart = DateTime.Now;
        Dictionary<string, GenericDescription>? ret = await schedulesDirectAPI.GetApiResponse<Dictionary<string, GenericDescription>?>(APIMethod.POST, "metadata/description/", request);
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved {ret.Count}/{request.Length} generic program descriptions. ({DateTime.Now - dtStart:G})");
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for {request.Length} generic program descriptions. ({DateTime.Now - dtStart:G})");
        }

        return ret;
    }

}