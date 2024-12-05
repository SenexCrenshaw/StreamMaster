namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IXmltvProgramBuilder
    {
        XmltvProgramme BuildXmltvProgram(MxfScheduleEntry scheduleEntry, string channelId, int timeShift, string baseUrl);
    }
}