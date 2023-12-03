using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterDomain.Services;

using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Data;

public partial class SchedulesDirectData(ILogger<SchedulesDirectData> logger, ISettingsService settingsService, IMemoryCache memoryCache) : ISchedulesDirectData
{
    [XmlAttribute("provider")]
    public string Provider { get; set; } = string.Empty;

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
