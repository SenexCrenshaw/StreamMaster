namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{
    public List<StationChannelName> GetStationChannelNames()
    { 
        var stations = schedulesDirectData.Services;
        List<StationChannelName> ret = [];

        foreach (var station in stations)
        {
            string channelNameSuffix = station.CallSign;

            var stationChannelName = new StationChannelName
            {
                Channel = station.StationId,
                DisplayName = station.CallSign+";"+station.Name,
                ChannelName = station.CallSign
            };
            ret.Add(stationChannelName);
        }

        return ret.OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase).ToList();
    }
}