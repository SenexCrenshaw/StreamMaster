using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Services;

using System.Collections.Concurrent;

namespace StreamMaster.SchedulesDirectAPI.Data;

public class SchedulesDirectDataService(ILogger<SchedulesDirectData> logger, ISettingsService settingsService, IMemoryCache memoryCache) : ISchedulesDirectDataService
{
    private readonly ConcurrentDictionary<int, SchedulesDirectData> schedulesDirectDatas = new();

    public List<MxfService> GetAllServices
    {
        get
        {
            List<MxfService> services = schedulesDirectDatas.Values.SelectMany(d => d.Services).ToList();
            return services;
        }
    }

    public List<MxfProgram> GetAllPrograms
    {
        get
        {
            List<MxfProgram> programs = schedulesDirectDatas.Values.SelectMany(d => d.Programs).ToList();
            return programs;
        }
    }

    public ISchedulesDirectData GetSchedulesDirectData(int ePGID)
    {
        return schedulesDirectDatas.GetOrAdd(ePGID, (epgId) =>
        {
            SchedulesDirectData data = new(logger, settingsService, memoryCache, ePGID);
            return data;
        });
    }

    public MxfService? GetService(string stationId)
    {
        MxfService? ret = GetAllServices.FirstOrDefault(s => s.StationId == stationId);
        return ret;
    }
}
