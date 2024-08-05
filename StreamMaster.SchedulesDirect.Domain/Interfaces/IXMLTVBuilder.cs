namespace StreamMaster.SchedulesDirect.Domain.Interfaces;
public interface IXMLTVBuilder
{
    XMLTV? CreateSDXmlTv(string baseUrl);
    XMLTV? CreateXmlTv(string baseUrl, List<VideoStreamConfig> videoStreamConfigs, OutputProfileDto outputProfile);
}