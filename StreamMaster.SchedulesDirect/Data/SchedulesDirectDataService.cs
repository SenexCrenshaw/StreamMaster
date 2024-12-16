using System.Collections.Concurrent;

using StreamMaster.Domain.Helpers;

namespace StreamMaster.SchedulesDirect.Data;

public class SchedulesDirectDataService()
    : ISchedulesDirectDataService
{
    public ConcurrentDictionary<int, ISchedulesDirectData> SchedulesDirectDatas { get; } = new();
    public ConcurrentDictionary<int, ICustomStreamData> CustomStreamDatas { get; set; } = new();

    public void Reset(int? EPGNumber = null)
    {
        if (EPGNumber.HasValue)
        {
            _ = SchedulesDirectDatas.TryRemove(EPGNumber.Value, out _);
        }
        else
        {
            SchedulesDirectDatas.Clear();
        }
    }

    public List<MxfService> AllServices
    {
        get
        {
            IEnumerable<MxfService> services = SchedulesDirectDatas.Values.SelectMany(d => d.Services.Values);
            return [.. services, .. CustomStreamDatas.Values.SelectMany(d => d.Services.Values)];
        }
    }

    public List<MxfLineup> AllLineups
    {
        get
        {
            List<MxfLineup> lineups = [.. SchedulesDirectDatas.Values.SelectMany(d => d.Lineups.Values)];
            return lineups;
        }
    }

    public ISchedulesDirectData SchedulesDirectData =>

         SchedulesDirectDatas.GetOrAdd(EPGHelper.SchedulesDirectId, (_) =>
        {
            SchedulesDirectData data = new(EPGHelper.SchedulesDirectId)
            {
                EPGNumber = EPGHelper.SchedulesDirectId
            };
            return data;
        });

    public IEnumerable<StationChannelName> GetStationChannelNames()
    {
        List<StationChannelName> ret = [];

        foreach (MxfService station in AllServices.Where(a => !a.StationId.StartsWith("DUMMY-")))
        {
            string channelNameSuffix = station.CallSign;

            StationChannelName stationChannelName = new(station.StationId, $"[{station.CallSign}] {station.Name}", station.CallSign, "", EPGHelper.SchedulesDirectId);

            ret.Add(stationChannelName);
        }

        return ret.OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase);
    }

    public ICustomStreamData CustomStreamData()
    {
        return CustomStreamDatas.GetOrAdd(EPGHelper.CustomPlayListId, (_) =>
        {
            CustomStreamData data = new(EPGHelper.CustomPlayListId)
            {
                EPGNumber = EPGHelper.CustomPlayListId
            };
            return data;
        });
    }

    public void ClearCustom()
    {
        CustomStreamDatas.Clear();
    }

    public List<MxfService> GetAllSDServices
    {
        get
        {
            List<MxfService> services = [.. SchedulesDirectData.Services.Select(a => a.Value)];
            return services;
        }
    }
}
