using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Data;

public partial class SchedulesDirectData
{
    [XmlIgnore] public List<MxfSeriesInfo> SeriesInfosToProcess { get; set; } = [];

    private Dictionary<string, MxfSeriesInfo> _seriesInfos = [];
    public MxfSeriesInfo FindOrCreateSeriesInfo(string seriesId, string? protoTypicalProgram = null, int? epgId = null)
    {
        if (_seriesInfos.TryGetValue(seriesId, out MxfSeriesInfo? seriesInfo))
        {
            return seriesInfo;
        }

        seriesInfo = new MxfSeriesInfo(SeriesInfos.Count + 1, seriesId, protoTypicalProgram);

        if (epgId != null)
        {
            seriesInfo.extras.Add("epgid", epgId);
        }

        SeriesInfos.Add(seriesInfo);
        _seriesInfos.Add(seriesId, seriesInfo);
        SeriesInfosToProcess.Add(seriesInfo);
        return seriesInfo;
    }
}