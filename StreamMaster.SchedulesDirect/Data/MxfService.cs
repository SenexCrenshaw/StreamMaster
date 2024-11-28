using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlArrayItem("Service")]
    public ConcurrentDictionary<string, MxfService> Services { get; set; } = [];

    public MxfService FindOrCreateService(string stationId)
    {
        MxfService service = Services.FindOrCreate(stationId, _ => new MxfService(Services.Count + 1, stationId)
        {
            EPGNumber = EPGNumber
        });

        return service;
    }

    public void RemoveService(string stationId)
    {
        Services.TryRemove(stationId, out _);
    }

    public MxfService? FindService(string stationId)
    {
        return Services.TryGetValue(stationId, out MxfService? service) ? service : null;
    }
}