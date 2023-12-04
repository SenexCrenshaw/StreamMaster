using StreamMaster.SchedulesDirectAPI.Domain.XmltvXml;

namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces
{
    public interface IXmltv2Mxf
    {
        XMLTV ConvertToMxf(string filepath, string lineupName);
        XMLTV ConvertToMxf(XMLTV xmltv, string lineupName);

    }
}