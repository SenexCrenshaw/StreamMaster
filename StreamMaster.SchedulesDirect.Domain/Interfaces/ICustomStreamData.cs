using System.Collections.Concurrent;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

public interface ICustomStreamData
{
    ConcurrentDictionary<string, MxfService> Services { get; set; }
    MxfService FindOrCreateService(string stationId);

    MxfService? GetService(string stationId);

    void RemoveService(string stationId);
    void ResetLists();
}
