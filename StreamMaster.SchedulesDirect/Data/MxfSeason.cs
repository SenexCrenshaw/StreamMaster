using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlArrayItem("Season")]
    public ConcurrentDictionary<string, Season> Seasons { get; set; } = new();

    [XmlIgnore]
    public List<Season> SeasonsToProcess { get; set; } = [];

    public Season FindOrCreateSeason(string seriesId, int seasonNumber, string protoTypicalProgram)
    {
        (Season season, bool created) = Seasons.FindOrCreateWithStatus($"{seriesId}_{seasonNumber}", key => new Season(Seasons.Count + 1, FindOrCreateSeriesInfo(seriesId), seasonNumber, protoTypicalProgram));

        if (created)
        {
            season.ProtoTypicalProgram ??= protoTypicalProgram;
            return season;
        }
        SeasonsToProcess.Add(season);
        return season;
    }

}
