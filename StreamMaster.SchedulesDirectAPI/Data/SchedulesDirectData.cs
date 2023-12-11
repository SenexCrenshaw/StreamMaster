using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Services;

using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Data;

public partial class SchedulesDirectData(ILogger<SchedulesDirectData> logger, ISettingsService settingsService, IMemoryCache memoryCache) : ISchedulesDirectData
{
    [XmlArrayItem("Provider")]
    public List<MxfProvider> Providers { get; set; }

    [XmlAttribute("provider")]
    public string Provider { get; set; } = string.Empty;
    [XmlArrayItem("Keyword")]
    public List<MxfKeyword> Keywords { get; set; } = [];
    public bool ShouldSerializeKeywords()
    {
        Keywords = Keywords?.OrderBy(k => k.GrpIndex).ThenBy(k => k.Id).ToList();
        return true;
    }

    [XmlArrayItem("KeywordGroup")]
    public List<MxfKeywordGroup> KeywordGroups { get; set; } = [];
    public bool ShouldSerializeKeywordGroups()
    {
        KeywordGroups = KeywordGroups?.OrderBy(k => k.Index).ThenBy(k => k.Uid).ToList();
        return true;
    }

    public void ResetLists()
    {
        GuideImages = [];
        People = [];
        SeriesInfos = [];
        Seasons = [];
        Programs = [];
        Affiliates = [];
        Services = [];
        ScheduleEntries = [];
        Lineups = [];
        ProgramsToProcess = [];
        SeriesInfosToProcess = [];
        SeasonsToProcess = [];
        ServicesToProcess = [];

        _affiliates = [];
        _guideImages = [];
        _keywordGroups = [];
        _lineups = [];
        _people = [];
        _programs = [];
        _seasons = [];
        _seriesInfos = [];
        _services = [];
    }


    [XmlArrayItem("GuideImage")]
    public List<MxfGuideImage> GuideImages { get; set; } = [];

    [XmlArrayItem("Person")]
    public List<MxfPerson> People { get; set; } = [];

    [XmlArrayItem("SeriesInfo")]
    public List<MxfSeriesInfo> SeriesInfos { get; set; } = [];

    [XmlArrayItem("Season")]
    public List<MxfSeason> Seasons { get; set; } = [];

    [XmlArrayItem("Program")]
    public List<MxfProgram> Programs { get; set; } = [];

    [XmlArrayItem("Affiliate")]
    public List<MxfAffiliate> Affiliates { get; set; } = [];

    [XmlArrayItem("Service")]
    public List<MxfService> Services { get; set; } = [];

    [XmlElement("ScheduleEntries")]
    public List<MxfScheduleEntries> ScheduleEntries { get; set; } = [];

    [XmlArrayItem("Lineup")]
    public List<MxfLineup> Lineups { get; set; } = [];
}
