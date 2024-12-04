
using System.Collections.Concurrent;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;
public interface ISchedulesDirectData
{
    //SeriesInfo? FindSeriesInfo(string seriesId);
    //ConcurrentDictionary<string, MxfProgram> ProgramsToProcess { get; set; }
    //ConcurrentDictionary<string, SeriesInfo> SeriesInfosToProcess { get; set; }
    //ConcurrentDictionary<string, Season> SeasonsToProcess { get; set; }

    //ConcurrentDictionary<string, MxfAffiliate> Affiliates { get; set; }
    //ConcurrentBag<MxfKeyword> Keywords { get; set; }
    //ConcurrentDictionary<string, MxfKeywordGroup> KeywordGroups { get; set; }
    ConcurrentDictionary<string, MxfLineup> Lineups { get; set; }
    //ConcurrentDictionary<string, MxfPerson> People { get; set; }

    //ConcurrentBag<MxfProvider> Providers { get; set; }
    //ConcurrentDictionary<string, Season> Seasons { get; set; }
    //ConcurrentDictionary<string, SeriesInfo> SeriesInfos { get; set; }
    ConcurrentDictionary<string, MxfService> Services { get; set; }

    //MxfPerson FindOrCreatePerson(string name);
    //Season FindOrCreateSeason(string seriesId, int seasonNumber, string ProgramId);
    //SeriesInfo FindOrCreateSeriesInfo(string seriesId, string ProgramId);
    //MxfKeywordGroup FindOrCreateKeywordGroup(KeywordGroupsEnum groupEnum, bool overflow = false);
    //MxfProgram FindOrCreateProgram(string programId);

    //MxfAffiliate FindOrCreateAffiliate(string affiliateName);
    MxfLineup FindOrCreateLineup(string lineupId, string lineupName);
    MxfService FindOrCreateService(string stationId);
    MxfService? FindService(string stationId);

    void RemoveLineup(string lineup);

    void RemoveService(string stationId);
    //void ResetLists();
}