using StreamMaster.SchedulesDirect.Domain.Models;
using StreamMaster.SchedulesDirect.Domain.XmltvXml;

using System.Collections.Concurrent;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

public interface ISchedulesDirectDataService
{
    List<StationChannelName> GetStationChannelNames();
    public ConcurrentDictionary<int, ISchedulesDirectData> SchedulesDirectDatas { get; }
    void Reset(int? epgId = null);
    XMLTV? CreateXmlTv(string baseUrl, List<VideoStreamConfig> videoStreamConfigs);
    List<MxfKeyword> AllKeywords { get; }
    List<MxfLineup> AllLineups { get; }
    List<MxfSeriesInfo> AllSeriesInfos { get; }
    List<MxfService> AllServices { get; }
    List<MxfProgram> AllPrograms { get; }

    ISchedulesDirectData GetSchedulesDirectData(int ePGID);
    MxfService? GetService(string stationId);
}