using StreamMaster.SchedulesDirect.Domain.XmltvXml;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IXmltv2Mxf
    {
        XMLTV? ConvertToMxf(string filepath, int EPGNumber);
    }
}