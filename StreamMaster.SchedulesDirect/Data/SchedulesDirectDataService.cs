using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Services;

using System.Collections.Concurrent;

namespace StreamMaster.SchedulesDirect.Data;

public class SchedulesDirectDataService(ILogger<SchedulesDirectData> logger, ISettingsService settingsService, IMemoryCache memoryCache) : ISchedulesDirectDataService
{
    public ConcurrentDictionary<int, ISchedulesDirectData> SchedulesDirectDatas { get; private set; } = new();

    public void Reset(int? epgId = null)
    {
        if (epgId.HasValue)
        {
            SchedulesDirectDatas.TryRemove(epgId.Value, out _);
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
            List<MxfService> services = SchedulesDirectDatas.Values.SelectMany(d => d.Services).ToList();
            return services;
        }
    }

    public List<MxfProgram> AllPrograms
    {
        get
        {
            List<MxfProgram> programs = SchedulesDirectDatas.Values.SelectMany(d => d.Programs).ToList();
            return programs;
        }
    }

    public ISchedulesDirectData GetSchedulesDirectData(int ePGID)
    {
        return SchedulesDirectDatas.GetOrAdd(ePGID, (epgId) =>
        {
            SchedulesDirectData data = new(logger, settingsService, memoryCache, ePGID);
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
                Channel = station.StationId,
                DisplayName = $"[{station.CallSign}] {station.Name}",
                ChannelName = station.CallSign
            };
            ret.Add(stationChannelName);
        }

        return [.. ret.OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase)];
    }
}
