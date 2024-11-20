namespace StreamMaster.SchedulesDirect;
public partial class SchedulesDirect
{
    public async Task<StationChannelMap?> GetStationChannelMapAsync(string lineup)
    {
        StationChannelMap? ret = await schedulesDirectAPI.GetApiResponse<StationChannelMap?>(APIMethod.GET, $"lineups/{lineup}");
        if (ret != null)
        {
            logger.LogDebug("Successfully retrieved the station mapping for lineup {lineup}. ({ret.Stations.Count} stations; {ret.Map.Count} channels)", lineup, ret.Stations.Count, ret.Map.Count);
        }
        else
        {
            logger.LogError("Did not receive a response from Schedules Direct for retrieval of lineup {lineup}.", lineup);
        }

        return ret;
    }
}