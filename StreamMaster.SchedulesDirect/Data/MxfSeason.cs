using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlArrayItem("Season")]
    public ConcurrentDictionary<string, Season> Seasons { get; set; } = new();

    [XmlIgnore]
    public ConcurrentDictionary<string, Season> SeasonsToProcess { get; set; } = [];

    public Season FindOrCreateSeason(string seriesId, int seasonNumber, string? protoTypicalProgram)
    {
        SeriesInfo seasonInfo = FindOrCreateSeriesInfo(seriesId, protoTypicalProgram);
        (Season season, bool created) = Seasons.FindOrCreateWithStatus($"{seriesId}_{seasonNumber}", _ => new Season(Seasons.Count + 1, seasonInfo, seasonNumber, protoTypicalProgram));

        SeasonsToProcess.TryAdd(season.Id, season);
        return season;
    }
}
