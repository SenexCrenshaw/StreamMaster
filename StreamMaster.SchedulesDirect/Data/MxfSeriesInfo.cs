using System.Collections.Concurrent;
using System.Xml.Serialization;

using SeriesInfo = StreamMaster.SchedulesDirect.Domain.Models.SeriesInfo;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlIgnore] public ConcurrentDictionary<string, SeriesInfo> SeriesInfosToProcess { get; set; } = [];

    [XmlArrayItem("SeriesInfo")]
    public ConcurrentDictionary<string, SeriesInfo> SeriesInfos { get; set; } = [];

    public SeriesInfo FindOrCreateSeriesInfo(string seriesId, string? ProgramId = null)
    {
        (SeriesInfo seriesInfo, bool created) = SeriesInfos.FindOrCreateWithStatus(seriesId, _ => new SeriesInfo(SeriesInfos.Count + 1, seriesId, ProgramId));


        SeriesInfosToProcess.TryAdd(seriesId, seriesInfo);
        return seriesInfo;
    }
}