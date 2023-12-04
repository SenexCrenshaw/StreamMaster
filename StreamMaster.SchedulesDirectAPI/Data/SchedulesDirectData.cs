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
