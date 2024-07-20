using StreamMaster.Domain.Helpers;

using System.Collections.Concurrent;

namespace StreamMaster.SchedulesDirect.Data;

public class SchedulesDirectDataService : ISchedulesDirectDataService
{
    private readonly ILogger<SchedulesDirectData> logger;

    public SchedulesDirectDataService(ILogger<SchedulesDirectData> logger)
    {
        this.logger = logger;

        _ = DummyData();
    }

    public ConcurrentDictionary<int, ISchedulesDirectData> SchedulesDirectDatas { get; private set; } = new();

    public ConcurrentDictionary<int, ICustomStreamData> CustomStreamDatas { get; private set; } = new();

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

    public void Set(int EPGNumber, ISchedulesDirectData schedulesDirectData)
    {
        _ = SchedulesDirectDatas.AddOrUpdate(EPGNumber, schedulesDirectData, (key, oldValue) => schedulesDirectData);
    }

    public List<MxfService> AllServices
    {
        get
        {            
            List<MxfService> services = SchedulesDirectDatas.Values.SelectMany(d => d.Services.Values).ToList();
            return services.Concat(CustomStreamDatas.Values.SelectMany(d => d.Services.Values)).ToList();
        }
    }

    public List<MxfProgram> AllPrograms
    {
        get
        {
            List<MxfProgram> programs = SchedulesDirectDatas.Values.SelectMany(d => d.Programs.Values).ToList();
            return programs;
        }
    }

    public List<MxfKeyword> AllKeywords
    {
        get
        {
            List<MxfKeyword> keywords = SchedulesDirectDatas.Values.SelectMany(d => d.Keywords).ToList();
            return keywords;
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

    public List<MxfSeriesInfo> AllSeriesInfos
    {
        get
        {
            List<MxfSeriesInfo> seriesInfo = SchedulesDirectDatas.Values.SelectMany(d => d.SeriesInfos.Values).ToList();
            return seriesInfo;
        }
    }

    public ISchedulesDirectData GetEPGData(int EPGNumber)
    {
        return SchedulesDirectDatas.GetOrAdd(EPGNumber, (epgId) =>
        {
            SchedulesDirectData data = new(logger, EPGNumber);
            return data;
        });
    }

    public ISchedulesDirectData SchedulesDirectData()
    {
        return SchedulesDirectDatas.GetOrAdd(EPGHelper.SchedulesDirectId, (epgId) =>
        {
            SchedulesDirectData data = new(logger, EPGHelper.SchedulesDirectId)
            {
                EPGNumber = EPGHelper.SchedulesDirectId
            };
            return data;
        });
    }

    public ISchedulesDirectData DummyData()
    {
        List<KeyValuePair<int, ISchedulesDirectData>> test = SchedulesDirectDatas.Where(a => a.Key == EPGHelper.DummyId).ToList();


        return SchedulesDirectDatas.GetOrAdd(EPGHelper.DummyId, (epgId) =>
        {
            SchedulesDirectData data = new(logger, EPGHelper.DummyId)
            {
                EPGNumber = EPGHelper.DummyId
            };

            MxfService mxfService = data.FindOrCreateService($"{EPGHelper.DummyId}-DUMMY");
            mxfService.CallSign = "Dummy";
            mxfService.Name = "Dummy EPG";

            return data;
        });
    }

    public MxfService? GetService(string stationId)
    {
        MxfService? ret = AllServices.FirstOrDefault(s => s.StationId == stationId);
        return ret;
    }

    public IEnumerable<StationChannelName> GetStationChannelNames()
    {

        List<StationChannelName> ret = [];

        foreach (MxfService station in AllServices.Where(a => !a.StationId.StartsWith("DUMMY-")))
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

        return ret.OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase);
    }

    public void ChangeServiceEPGNumber(int oldEPGNumber, int newEPGNumber)
    {
        foreach (MxfService service in GetEPGData(oldEPGNumber).Services.Values)
        {
            service.EPGNumber = newEPGNumber;
        }
    }


    public ICustomStreamData CustomStreamData()
    {
        return CustomStreamDatas.GetOrAdd(EPGHelper.CustomPlayListId, (epgId) =>
        {
            CustomStreamData data = new(logger, EPGHelper.CustomPlayListId)
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

    public List<MxfProgram> GetAllSDPrograms
    {
        get
        {
            List<MxfProgram> programs = SchedulesDirectData().Programs.Select(a => a.Value).ToList();
            return programs;
        }
    }

    public List<MxfService> GetAllSDServices
    {
        get
        {
            List<MxfService> services = SchedulesDirectData().Services.Select(a => a.Value).ToList();
            return services;
        }
    }
}

