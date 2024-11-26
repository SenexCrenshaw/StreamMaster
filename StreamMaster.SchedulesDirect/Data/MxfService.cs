using System.Collections.Concurrent;
using System.Xml.Serialization;

using StreamMaster.Domain.Models;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    //[XmlIgnore] public List<MxfService> ServicesToProcess = [];

    [XmlArrayItem("Service")]
    public ConcurrentDictionary<string, MxfService> Services { get; set; } = [];

    //[XmlElement("ScheduleEntries")]
    //public List<MxfScheduleEntries> ScheduleEntries { get; set; } = [];

    public MxfService FindOrCreateService(string stationId)
    {
        (MxfService service, bool created) = Services.FindOrCreateWithStatus(stationId, _ => new MxfService(Services.Count + 1, stationId)
        {
            EPGNumber = EPGNumber
        });

        return service;
    }

    public async Task<MxfService> FindOrCreateDummyService(string stationId, VideoStreamConfig videoStreamConfig)
    {
        if (!Services.ContainsKey(stationId))
        {
            await WriteToCSVAsync(serviceCSV, $"FindOrCreateService: {stationId}");
        }
        (MxfService service, bool created) = Services.FindOrCreateWithStatus(stationId, _ => new MxfService(Services.Count + 1, stationId)
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
                //service.LogoImage = videoStreamConfig.Logo;
                service.extras.TryAdd("logo", new StationImage
                {
                    Url = videoStreamConfig.Logo
                });

                //service.mxfGuideImage = FindOrCreateProgramArtwork(videoStreamConfig.User_Tvg_Logo);
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