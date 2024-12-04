namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    //[XmlArrayItem("Season")]
    //public ConcurrentDictionary<string, Season> Seasons { get; set; } = new();

    //[XmlIgnore]
    //public ConcurrentDictionary<string, Season> SeasonsToProcess { get; set; } = [];

    //public Season FindOrCreateSeason(string seriesId, int seasonNumber, string ProgramId)
    //{
    //    SeriesInfo seasonInfo = FindOrCreateSeriesInfo(seriesId, ProgramId);
    //    Season season = Seasons.FindOrCreate($"{seriesId}_{seasonNumber}", _ => new Season(Seasons.Count + 1, seasonInfo, seasonNumber, ProgramId));

    //    SeasonsToProcess.TryAdd(season.Id, season);
    //    return season;
    //}
}
