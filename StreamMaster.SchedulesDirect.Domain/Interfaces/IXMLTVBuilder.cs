using StreamMaster.Domain.Models;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;
public interface IXMLTVBuilder
{
    XMLTV? CreateSDXmlTv(string baseUrl);
    Task<XMLTV?> CreateXmlTv(List<VideoStreamConfig> videoStreamConfigs);
}