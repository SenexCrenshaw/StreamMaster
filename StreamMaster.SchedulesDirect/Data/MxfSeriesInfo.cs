using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlIgnore] public List<MxfSeriesInfo> SeriesInfosToProcess { get; set; } = [];

    private Dictionary<string, MxfSeriesInfo> _seriesInfos = [];
    public MxfSeriesInfo FindOrCreateSeriesInfo(string seriesId, string? protoTypicalProgram = null)
    {
        if (_seriesInfos.TryGetValue(seriesId, out MxfSeriesInfo? seriesInfo))
        {
            return seriesInfo;
        }

        seriesInfo = new MxfSeriesInfo(SeriesInfos.Count + 1, seriesId, protoTypicalProgram);


        SeriesInfos.Add(seriesInfo);
        _seriesInfos.Add(seriesId, seriesInfo);
        SeriesInfosToProcess.Add(seriesInfo);
        return seriesInfo;
    }
}