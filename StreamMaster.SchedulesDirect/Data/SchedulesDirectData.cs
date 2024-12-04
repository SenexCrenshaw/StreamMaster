using StreamMaster.Domain.Cache;
using StreamMaster.SchedulesDirect.Images;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData(int EPGNumber, HybridCacheManager<MovieImages> movieCache, HybridCacheManager<EpisodeImages> episodeCache) : ISchedulesDirectData
{
    public int EPGNumber { get; set; } = EPGNumber;

    public void ResetLists()
    {
        Lineups.Clear();
        People.Clear();
        Programs.Clear();
        Seasons.Clear();
        SeasonsToProcess.Clear();
        SeriesInfos.Clear();
        SeriesInfosToProcess.Clear();
        Services.Clear();
    }
}
