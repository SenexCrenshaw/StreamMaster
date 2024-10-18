using StreamMaster.SchedulesDirect.Domain.Models;

using System.Collections.Concurrent;


namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

public interface ICustomStreamData
{

    //ConcurrentDictionary<string, MxfProgram> Programs { get; set; }

    ConcurrentDictionary<string, MxfService> Services { get; set; }

    //MxfProgram FindOrCreateProgram(string programId);

    MxfService FindOrCreateService(string stationId);

    MxfService? GetService(string stationId);

    //void RemoveProgram(string programId);
    void RemoveService(string stationId);
    void ResetLists();
}
