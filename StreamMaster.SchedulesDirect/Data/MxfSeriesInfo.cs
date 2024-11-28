using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlIgnore] public ConcurrentDictionary<string, SeriesInfo> SeriesInfosToProcess { get; set; } = [];

    [XmlArrayItem("SeriesInfo")]
    public ConcurrentDictionary<string, SeriesInfo> SeriesInfos { get; set; } = [];

    public SeriesInfo FindOrCreateSeriesInfo(string seriesId, string? ProgramId = null)
    {
        SeriesInfo seriesInfo = new(seriesId, ProgramId);
        SeriesInfosToProcess.TryAdd(seriesId, new(seriesId, ProgramId));
        return seriesInfo;
    }
}