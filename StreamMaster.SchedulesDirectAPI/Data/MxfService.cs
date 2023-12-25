using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Data;

public partial class SchedulesDirectData
{
    [XmlIgnore] public List<MxfService> ServicesToProcess = [];

    private Dictionary<string, MxfService> _services = [];
    public MxfService FindOrCreateService(string stationId, int? epgId = null)
    {
        if (_services.TryGetValue(stationId, out MxfService? service))
        {
            return service;
        }

        service = new MxfService(Services.Count + 1, stationId);
        if (epgId != null)
        {
            service.extras.Add("epgid", epgId);
            foreach (MxfScheduleEntry s in service.MxfScheduleEntries.ScheduleEntry)
            {
                s.extras.Add("epgid", epgId);
            }
        }

        Services.Add(service);

        ScheduleEntries.Add(service.MxfScheduleEntries);
        _services.Add(stationId, service);
        ServicesToProcess.Add(service);
        return service;
    }

    public void RemoveService(string stationId)
    {
        if (!_services.TryGetValue(stationId, out MxfService? service))
        {
            return;
        }
        _services.Remove(stationId);
        Services.Remove(service);
    }

    public MxfService? GetService(string stationId)
    {
        return _services.TryGetValue(stationId, out MxfService? service) ? service : null;
    }

}