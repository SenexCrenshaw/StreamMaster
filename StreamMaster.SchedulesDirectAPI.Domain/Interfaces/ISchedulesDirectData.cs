using StreamMaster.SchedulesDirectAPI.Domain.Models;

using System.Xml.Serialization;


namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces
{
    public interface ISchedulesDirectData
    {
        void ResetLists();
        MxfService? GetService(string stationId);
        [XmlArrayItem("Lineup")]
        public List<MxfLineup> Lineups { get; set; }
        [XmlArrayItem("SeriesInfo")]
        public List<MxfSeriesInfo> SeriesInfos { get; set; }
        public List<MxfProvider> Providers { get; set; }
        List<MxfKeywordGroup> KeywordGroups { get; set; }
        List<MxfKeyword> Keywords { get; set; }
        List<MxfSeason> SeasonsToProcess { get; set; }
        List<MxfProgram> ProgramsToProcess { get; set; }
        List<MxfProgram> Programs { get; set; }
        List<MxfService> Services { get; set; }
        List<MxfSeriesInfo> SeriesInfosToProcess { get; set; }
        MxfPerson FindOrCreatePerson(string name, int? epgId = null);
        MxfSeason FindOrCreateSeason(string seriesId, int seasonNumber, string protoTypicalProgram, int? epgId = null);
        MxfSeriesInfo FindOrCreateSeriesInfo(string seriesId, string? protoTypicalProgram = null, int? epgId = null);
        MxfKeywordGroup FindOrCreateKeywordGroup(KeywordGroupsEnum groupEnum, bool overflow = false, int? epgId = null);
        MxfProgram FindOrCreateProgram(string programId, int? epgId = null);
        MxfAffiliate FindOrCreateAffiliate(string affiliateName);
        MxfGuideImage FindOrCreateGuideImage(string pathname, int? epgId = null);
        MxfLineup FindOrCreateLineup(string lineupId, string lineupName, int? epgId = null);
        MxfService FindOrCreateService(string stationId, int? epgId = null);
        void RemoveProgram(string programId);
        void RemoveService(string stationId);
    }
}