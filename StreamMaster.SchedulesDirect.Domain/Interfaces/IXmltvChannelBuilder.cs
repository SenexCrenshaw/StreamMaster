using StreamMaster.Domain.Models;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IXmltvChannelBuilder
    {
        XmltvChannel BuildXmltvChannel(MxfService service, bool isOG);
        XmltvChannel BuildXmltvChannel(XmltvChannel xmltvChannel, VideoStreamConfig videoStreamConfig);
    }
}