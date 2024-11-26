
using System.Collections.Concurrent;

using StreamMaster.Domain.Models;
using StreamMaster.SchedulesDirect.Domain.Enums;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;
public interface ISchedulesDirectData
{
    ConcurrentDictionary<string, MxfAffiliate> Affiliates { get; set; }
    ConcurrentDictionary<string, ProgramArtwork> GuideImages { get; set; }
    ConcurrentBag<MxfKeyword> Keywords { get; set; }
    ConcurrentDictionary<string, MxfKeywordGroup> KeywordGroups { get; set; }
    ConcurrentDictionary<string, MxfLineup> Lineups { get; set; }
    ConcurrentDictionary<string, MxfPerson> People { get; set; }
    ConcurrentDictionary<string, MxfProgram> Programs { get; set; }
    List<MxfProgram> ProgramsToProcess { get; set; }

    ConcurrentBag<MxfProvider> Providers { get; set; }

    ConcurrentDictionary<string, Season> Seasons { get; set; }
    ConcurrentDictionary<string, SeriesInfo> SeriesInfos { get; set; }
    List<SeriesInfo> SeriesInfosToProcess { get; set; }
    //List<MxfScheduleEntries> ScheduleEntries { get; set; }
    List<Season> SeasonsToProcess { get; set; }

    ConcurrentDictionary<string, MxfService> Services { get; set; }
    void RemoveLineup(string lineup);

    MxfPerson FindOrCreatePerson(string name);
    Season FindOrCreateSeason(string seriesId, int seasonNumber, string? protoTypicalProgram);
    SeriesInfo FindOrCreateSeriesInfo(string seriesId, string? protoTypicalProgram = null);
    MxfKeywordGroup FindOrCreateKeywordGroup(KeywordGroupsEnum groupEnum, bool overflow = false);
    MxfProgram FindOrCreateProgram(string programId);
    MxfAffiliate FindOrCreateAffiliate(string affiliateName);
    ProgramArtwork? FindOrCreateProgramArtwork(ProgramArtwork artwork);
    MxfLineup FindOrCreateLineup(string lineupId, string lineupName);
    MxfService FindOrCreateService(string stationId);
    Task<MxfService> FindOrCreateDummyService(string stationId, VideoStreamConfig videoStreamConfig);
    MxfService? GetService(string stationId);
    void RemoveProgram(string programId);
    void RemoveService(string stationId);
    void ResetLists();
}