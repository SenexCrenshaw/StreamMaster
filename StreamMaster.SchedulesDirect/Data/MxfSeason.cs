using StreamMaster.Domain.Extensions;

using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlArrayItem("Season")]
    public ConcurrentDictionary<string, MxfSeason> Seasons { get; set; } = new();

    [XmlIgnore]
    public List<MxfSeason> SeasonsToProcess { get; set; } = [];

    public MxfSeason FindOrCreateSeason(string seriesId, int seasonNumber, string protoTypicalProgram)
    {
        (MxfSeason season, bool created) = Seasons.FindOrCreateWithStatus($"{seriesId}_{seasonNumber}", key => new MxfSeason(Seasons.Count + 1, FindOrCreateSeriesInfo(seriesId), seasonNumber, protoTypicalProgram));

        if (created)
        {
            season.ProtoTypicalProgram ??= protoTypicalProgram;
            return season;
        }
        SeasonsToProcess.Add(season);
        return season;
    }
}
