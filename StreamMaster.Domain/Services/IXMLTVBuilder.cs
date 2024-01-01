using StreamMaster.SchedulesDirect.Domain.Models;
using StreamMaster.SchedulesDirect.Domain.XmltvXml;

namespace StreamMaster.Domain.Services;
public interface IXMLTVBuilder
{
    XMLTV? CreateXmlTv(string baseUrl, List<VideoStreamConfig> videoStreamConfigs);
}