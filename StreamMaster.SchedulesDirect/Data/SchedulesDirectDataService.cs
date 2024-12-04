using System.Collections.Concurrent;

using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Helpers;
using StreamMaster.SchedulesDirect.Images;

namespace StreamMaster.SchedulesDirect.Data;

public class SchedulesDirectDataService(HybridCacheManager<MovieImages> movieCache, HybridCacheManager<EpisodeImages> episodeCache)
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
            List<MxfLineup> lineups = SchedulesDirectDatas.Values.SelectMany(d => d.Lineups.Values).ToList();
            return lineups;
        }
    }

    //public ISchedulesDirectData GetEPGData(int EPGNumber)
    //{
    //    return SchedulesDirectDatas.GetOrAdd(EPGNumber, (_) =>
    //    {
    //        SchedulesDirectData data = new SchedulesDirectData(EPGNumber);
    //        return data;
    //    });
    //}

    public ISchedulesDirectData SchedulesDirectData =>

         SchedulesDirectDatas.GetOrAdd(EPGHelper.SchedulesDirectId, (_) =>
        {
            SchedulesDirectData data = new(EPGHelper.SchedulesDirectId, movieCache, episodeCache)
            {
                EPGNumber = EPGHelper.SchedulesDirectId
            };
            return data;
        });


    //public ISchedulesDirectData DummyData()
    //{
    //    List<KeyValuePair<int, ISchedulesDirectData>> test = SchedulesDirectDatas.Where(a => a.Key == EPGHelper.DummyId).ToList();

    //    return SchedulesDirectDatas.GetOrAdd(EPGHelper.DummyId, (_) =>
    //    {
    //        SchedulesDirectData data = new(EPGHelper.DummyId)
    //        {
    //            EPGNumber = EPGHelper.DummyId
    //        };

    //        MxfService mxfService = data.FindOrCreateService($"{EPGHelper.DummyId}-DUMMY");
    //        mxfService.CallSign = "Dummy";
    //        mxfService.Name = "Dummy EPG";

    //        return data;
    //    });
    //}

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
            List<MxfService> services = SchedulesDirectData.Services.Select(a => a.Value).ToList();
            return services;
        }
    }
}
