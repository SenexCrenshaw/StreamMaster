using StreamMaster.Domain.Extensions;

using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlIgnore] public List<MxfService> ServicesToProcess = [];

    [XmlArrayItem("Service")]
    public ConcurrentDictionary<string, MxfService> Services { get; set; } = [];

    [XmlElement("ScheduleEntries")]
    public List<MxfScheduleEntries> ScheduleEntries { get; set; } = [];

    public MxfService FindOrCreateService(string stationId)
    {
        (MxfService service, bool created) = Services.FindOrCreateWithStatus(stationId, key => new MxfService(Services.Count + 1, stationId)
        {
            EPGNumber = EPGNumber
        });
        if (created)
        {
            return service;
        }

        ScheduleEntries.Add(service.MxfScheduleEntries);
        ServicesToProcess.Add(service);
        return service;
    }

    public void RemoveService(string stationId)
    {
        Services.TryRemove(stationId, out _);
    }

    public MxfService? GetService(string stationId)
    {
        return Services.TryGetValue(stationId, out MxfService? service) ? service : null;
    }

}