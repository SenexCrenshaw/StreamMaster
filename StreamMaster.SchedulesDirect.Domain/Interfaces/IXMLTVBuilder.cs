using StreamMaster.Domain.Models;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;
public interface IXMLTVBuilder
{
    Task<XMLTV?> CreateXmlTv(List<VideoStreamConfig> videoStreamConfigs);
}