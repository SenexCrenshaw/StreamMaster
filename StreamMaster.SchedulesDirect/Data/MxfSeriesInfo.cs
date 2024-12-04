namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    //[XmlIgnore] public ConcurrentDictionary<string, SeriesInfo> SeriesInfosToProcess { get; set; } = [];

    //[XmlArrayItem("SeriesInfo")]
    //public ConcurrentDictionary<string, SeriesInfo> SeriesInfos { get; set; } = [];

    //public SeriesInfo FindOrCreateSeriesInfo(string seriesId, string ProgramId)
    //{
    //    SeriesInfo seriesInfo = new(seriesId, ProgramId);
    //    SeriesInfosToProcess.TryAdd(seriesId, new(seriesId, ProgramId));
    //    SeriesInfos.TryAdd(ProgramId, seriesInfo);
    //    return seriesInfo;
    //}

    //public SeriesInfo? FindSeriesInfo(string seriesId)
    //{
    //    return SeriesInfos.TryGetValue(seriesId, out SeriesInfo? seriesInfo) ? seriesInfo : null;

    //}
}