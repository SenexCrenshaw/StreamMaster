using System.Collections.Concurrent;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

public interface ISchedulesDirectDataService
{
    IEnumerable<StationChannelName> GetStationChannelNames();
    ConcurrentDictionary<int, ISchedulesDirectData> SchedulesDirectDatas { get; }
    void Reset(int? EPGNumber = null);
    List<MxfLineup> AllLineups { get; }
    List<MxfService> AllServices { get; }
    ISchedulesDirectData GetEPGData(int EPGNumber);
    ICustomStreamData CustomStreamData();
    ISchedulesDirectData SchedulesDirectData();
    ISchedulesDirectData DummyData();
    List<MxfService> GetAllSDServices { get; }
}
