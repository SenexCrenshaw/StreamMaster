using StreamMaster.SchedulesDirect.Domain.Models;

using System.Collections.Concurrent;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

public interface ISchedulesDirectDataService
{
    List<StationChannelName> GetStationChannelNames();
    public ConcurrentDictionary<int, ISchedulesDirectData> SchedulesDirectDatas { get; }
    void Reset(int? epgId = null);
    List<MxfService> AllServices { get; }
    List<MxfProgram> AllPrograms { get; }
    ISchedulesDirectData GetSchedulesDirectData(int ePGID);
    MxfService? GetService(string stationId);
}