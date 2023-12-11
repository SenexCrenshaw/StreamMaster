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
        MxfPerson FindOrCreatePerson(string name);
        MxfSeason FindOrCreateSeason(string seriesId, int seasonNumber, string protoTypicalProgram);
        MxfSeriesInfo FindOrCreateSeriesInfo(string seriesId, string? protoTypicalProgram = null);
        MxfKeywordGroup FindOrCreateKeywordGroup(KeywordGroupsEnum groupEnum, bool overflow = false);
        MxfProgram FindOrCreateProgram(string programId);
        MxfAffiliate FindOrCreateAffiliate(string affiliateName);
        MxfGuideImage FindOrCreateGuideImage(string pathname);
        MxfLineup FindOrCreateLineup(string lineupId, string lineupName);
        MxfService FindOrCreateService(string stationId);
        void RemoveProgram(string programId);
        void RemoveService(string stationId);
    }
}