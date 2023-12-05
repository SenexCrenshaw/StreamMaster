using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Data;

public partial class SchedulesDirectData
{
    [XmlIgnore] public List<MxfService> ServicesToProcess = [];

    private readonly Dictionary<string, MxfService> _services = [];
    public MxfService FindOrCreateService(string stationId)
    {
        if (_services.TryGetValue(stationId, out var service)) return service;
        Services.Add(service = new MxfService(Services.Count + 1, stationId));
        ScheduleEntries.Add(service.MxfScheduleEntries);
        _services.Add(stationId, service);
        ServicesToProcess.Add(service);
        return service;
    }

    public MxfService? GetService(string stationId)
    {
        return _services.TryGetValue(stationId, out var service) ? service : null;
    }

}