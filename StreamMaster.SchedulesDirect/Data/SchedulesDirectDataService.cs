using Microsoft.Extensions.Caching.Memory;

using StreamMaster.SchedulesDirect.Helpers;

using System.Collections.Concurrent;

namespace StreamMaster.SchedulesDirect.Data;

public class SchedulesDirectDataService(ILogger<SchedulesDirectData> logger, ILogger<EPGImportLogger> _epgImportLogger, IEPGHelper ePGHelper, IMemoryCache memoryCache) : ISchedulesDirectDataService
{

    public ConcurrentDictionary<int, ISchedulesDirectData> SchedulesDirectDatas { get; private set; } = new();
    public void Reset(int? EPGNumber = null)
    {
        if (EPGNumber.HasValue)
        {
            SchedulesDirectDatas.TryRemove(EPGNumber.Value, out _);
        }
        else
        {
            SchedulesDirectDatas.Clear();
        }

    }

    public void Set(int EPGNumber, ISchedulesDirectData schedulesDirectData)
    {
        SchedulesDirectDatas.AddOrUpdate(EPGNumber, schedulesDirectData, (key, oldValue) => schedulesDirectData);
    }

    public List<MxfService> AllServices
    {
        get
        {
            List<MxfService> services = SchedulesDirectDatas.Values.SelectMany(d => d.Services.Values).ToList();
            return services;
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
            SchedulesDirectData data = new(logger, _epgImportLogger, ePGHelper, memoryCache, EPGNumber);
            return data;
        });
    }

    public ISchedulesDirectData SchedulesDirectData()
    {
        return SchedulesDirectDatas.GetOrAdd(EPGHelper.SchedulesDirectId, (epgId) =>
        {
            SchedulesDirectData data = new(logger, _epgImportLogger, ePGHelper, memoryCache, EPGHelper.SchedulesDirectId)
            {
                EPGNumber = EPGHelper.SchedulesDirectId
            };
            return data;
        });
    }

    public ISchedulesDirectData DummyData()
    {
        return SchedulesDirectDatas.GetOrAdd(EPGHelper.DummyId, (epgId) =>
        {
            SchedulesDirectData data = new(logger, _epgImportLogger, ePGHelper, memoryCache, EPGHelper.DummyId)
            {
                EPGNumber = EPGHelper.DummyId
            };
            return data;
        });
    }

    public MxfService? GetService(string stationId)
    {
        MxfService? ret = AllServices.FirstOrDefault(s => s.StationId == stationId);
        return ret;
    }

    public List<StationChannelName> GetStationChannelNames()
    {

        List<StationChannelName> ret = [];

        foreach (MxfService station in AllServices.Where(a => !a.StationId.StartsWith("DUMMY-")))
        {
            string channelNameSuffix = station.CallSign;

            StationChannelName stationChannelName = new()
            {
                Channel = $"{station.EPGNumber}-{station.StationId}",
                DisplayName = $"[{station.CallSign}] {station.Name}",
                ChannelName = station.CallSign
            };
            ret.Add(stationChannelName);
        }

        return [.. ret.OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase)];
    }

    public void ChangeServiceEPGNumber(int oldEPGNumber, int newEPGNumber)
    {
        foreach (MxfService service in GetEPGData(oldEPGNumber).Services.Values)
        {
            service.EPGNumber = newEPGNumber;
        }
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

