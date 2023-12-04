using StreamMaster.SchedulesDirectAPI.Domain.Models;


namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces
{
    public interface ISchedulesDirectData
    {
        //List<MxfKeywordGroup> KeywordGroups { get; set; }
        List<MxfProgram> ProgramsToProcess { get; set; }
        List<MxfProgram> Programs { get; set; }
        List<MxfService> Services { get; set; }

        MxfPerson FindOrCreatePerson(string name);
        MxfSeason FindOrCreateSeason(string seriesId, int seasonNumber, string protoTypicalProgram);
        MxfSeriesInfo FindOrCreateSeriesInfo(string seriesId, string? protoTypicalProgram = null);
        MxfKeywordGroup FindOrCreateKeywordGroup(KeywordGroupsEnum groupEnum, bool overflow = false);
        MxfProgram FindOrCreateProgram(string programId);
        MxfAffiliate FindOrCreateAffiliate(string affiliateName);
        MxfGuideImage FindOrCreateGuideImage(string pathname);
        MxfLineup FindOrCreateLineup(string lineupId, string lineupName);
        MxfService FindOrCreateService(string stationId);
    }
}