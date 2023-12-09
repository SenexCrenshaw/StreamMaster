using StreamMaster.SchedulesDirectAPI.Domain.XmltvXml;

namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces
{
    public interface IXmltv2Mxf
    {
        XMLTV? ConvertToMxf(string filepath, int EPGId);

    }
}