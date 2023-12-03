using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Enums;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{ 
    public async Task<StationChannelMap?> GetStationChannelMapAsync(string lineup)
    {
        StationChannelMap? ret = await schedulesDirectAPI.GetApiResponse<StationChannelMap?>(APIMethod.GET, $"lineups/{lineup}");
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved the station mapping for lineup {lineup}. ({ret.Stations.Count} stations; {ret.Map.Count} channels)");
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for retrieval of lineup {lineup}.");
        }

        return ret;
    }

    public async Task<Dictionary<string, Dictionary<string, ScheduleMd5Response>>?> GetScheduleMd5sAsync(ScheduleRequest[] request)
    {
        DateTime dtStart = DateTime.Now;
        Dictionary<string, Dictionary<string, ScheduleMd5Response>>? ret = await schedulesDirectAPI.GetApiResponse<Dictionary<string, Dictionary<string, ScheduleMd5Response>>?>(APIMethod.POST, "schedules/md5", request);
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved Md5s for {ret.Count}/{request.Length} stations' daily schedules. ({DateTime.Now - dtStart:G})");
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for Md5s of {request.Length} stations' daily schedules. ({DateTime.Now - dtStart:G})");
        }

        return ret;
    }

    public async Task<List<ScheduleResponse>?> GetScheduleListingsAsync(ScheduleRequest[] request)
    {
        DateTime dtStart = DateTime.Now;
        List<ScheduleResponse>? ret = await schedulesDirectAPI.GetApiResponse<List<ScheduleResponse>?>(APIMethod.POST, "schedules", request);
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved {request.Length} stations' daily schedules. ({DateTime.Now - dtStart:G})");
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for {request.Length} stations' daily schedules. ({DateTime.Now - dtStart:G})");
        }

        return ret;
    }

    public async Task<List<ScheduleResponse>?> GetScheduleListings(ScheduleRequest[] request)
    {
        DateTime dtStart = DateTime.Now;
        List<ScheduleResponse>? ret = await schedulesDirectAPI.GetApiResponse<List<ScheduleResponse>>(APIMethod.POST, "schedules", request);
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved {request.Length} stations' daily schedules. ({DateTime.Now - dtStart:G})");
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for {request.Length} stations' daily schedules. ({DateTime.Now - dtStart:G})");
        }

        return ret;
    }
}