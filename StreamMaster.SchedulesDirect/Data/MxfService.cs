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
        //if (!Services.ContainsKey(stationId))
        //{
        //    WriteToCSV(serviceCSV, $"{Services.Count + 1},{stationId}");
        //}

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

    public MxfService FindOrCreateDummyService(string stationId, VideoStreamConfig videoStreamConfig)
    {
        if (!Services.ContainsKey(stationId))
        {
            WriteToCSV(serviceCSV, $"FindOrCreateService: {stationId}");
        }
        (MxfService service, bool created) = Services.FindOrCreateWithStatus(stationId, key => new MxfService(Services.Count + 1, stationId)
        {
            EPGNumber = EPGNumber
        });
        if (created)
        {
            service.Name = videoStreamConfig.Name;
            service.CallSign = videoStreamConfig.Name;
            service.ChNo = videoStreamConfig.ChannelNumber;
            if (!string.IsNullOrEmpty(videoStreamConfig.Logo) && videoStreamConfig.Logo.StartsWith("http"))
            {
                service.LogoImage = videoStreamConfig.Logo;
                service.extras.AddOrUpdate("logo", new StationImage
                {
                    Url = videoStreamConfig.Logo

                });

                //service.mxfGuideImage = FindOrCreateGuideImage(videoStreamConfig.User_Tvg_Logo);
            }

        }
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