using StreamMaster.SchedulesDirect.Domain.Models;

using System.Collections.Concurrent;


namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface ISchedulesDirectData
    {

        ConcurrentDictionary<string, MxfAffiliate> Affiliates { get; set; }
        ConcurrentDictionary<string, MxfGuideImage> GuideImages { get; set; }
        ConcurrentBag<MxfKeyword> Keywords { get; set; }
        ConcurrentDictionary<string, MxfKeywordGroup> KeywordGroups { get; set; }
        ConcurrentDictionary<string, MxfLineup> Lineups { get; set; }
        ConcurrentDictionary<string, MxfPerson> People { get; set; }

        ConcurrentDictionary<string, MxfProgram> Programs { get; set; }
        List<MxfProgram> ProgramsToProcess { get; set; }

        ConcurrentBag<MxfProvider> Providers { get; set; }

        ConcurrentDictionary<string, MxfSeason> Seasons { get; set; }
        ConcurrentDictionary<string, MxfSeriesInfo> SeriesInfos { get; set; }
        List<MxfSeriesInfo> SeriesInfosToProcess { get; set; }
        List<MxfScheduleEntries> ScheduleEntries { get; set; }
        List<MxfSeason> SeasonsToProcess { get; set; }


        ConcurrentDictionary<string, MxfService> Services { get; set; }

        void RemoveLineup(string lineup);

        MxfPerson FindOrCreatePerson(string name);
        MxfSeason FindOrCreateSeason(string seriesId, int seasonNumber, string protoTypicalProgram);
        MxfSeriesInfo FindOrCreateSeriesInfo(string seriesId, string? protoTypicalProgram = null);
        MxfKeywordGroup FindOrCreateKeywordGroup(KeywordGroupsEnum groupEnum, bool overflow = false);
        MxfProgram FindOrCreateProgram(string programId);
        MxfAffiliate FindOrCreateAffiliate(string affiliateName);
        MxfGuideImage FindOrCreateGuideImage(string pathname);
        MxfLineup FindOrCreateLineup(string lineupId, string lineupName);
        MxfService FindOrCreateService(string stationId);
        MxfService FindOrCreateDummyService(string stationId, VideoStreamConfig videoStreamConfig);
        MxfService? GetService(string stationId);

        void RemoveProgram(string programId);
        void RemoveService(string stationId);
        void ResetLists();
    }
}