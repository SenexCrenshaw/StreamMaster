using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{

    [XmlIgnore] public List<MxfSeriesInfo> SeriesInfosToProcess { get; set; } = [];

    [XmlArrayItem("SeriesInfo")]
    public ConcurrentDictionary<string, MxfSeriesInfo> SeriesInfos { get; set; } = [];

    public MxfSeriesInfo FindOrCreateSeriesInfo(string seriesId, string? protoTypicalProgram = null)
    {
        (MxfSeriesInfo seriesInfo, bool created) = SeriesInfos.FindOrCreateWithStatus(seriesId, key => new MxfSeriesInfo(SeriesInfos.Count + 1, seriesId, protoTypicalProgram));
        if (created)
        {
            return seriesInfo;
        }

        SeriesInfosToProcess.Add(seriesInfo);
        return seriesInfo;
    }
}