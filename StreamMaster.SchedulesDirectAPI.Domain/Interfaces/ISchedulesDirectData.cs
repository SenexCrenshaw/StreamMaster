using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces
{
    public interface ISchedulesDirectData
    {
        List<MxfService> Services { get; set; }
        MxfProgram FindOrCreateProgram(string programId);
        MxfAffiliate FindOrCreateAffiliate(string affiliateName);
        MxfGuideImage FindOrCreateGuideImage(string pathname);
        MxfLineup FindOrCreateLineup(string lineupId, string lineupName);
        MxfService FindOrCreateService(string stationId);
    }
}