namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{
    public List<StationChannelName> GetStationChannelNames()
    {
        List<MxfService> stations = schedulesDirectData.Services;
        List<StationChannelName> ret = [];

        foreach (MxfService station in stations)
        {
            string channelNameSuffix = station.CallSign;

            StationChannelName stationChannelName = new()
            {
                Channel = station.StationId,
                DisplayName = $"[{station.CallSign}] {station.Name}",
                ChannelName = station.CallSign
            };
            ret.Add(stationChannelName);
        }

        return [.. ret.OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase)];
    }
}