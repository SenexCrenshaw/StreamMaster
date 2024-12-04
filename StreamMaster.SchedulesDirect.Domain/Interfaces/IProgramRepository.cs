using System.Collections.Concurrent;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IProgramRepository
    {
        ConcurrentDictionary<string, MxfProgram> Programs { get; }
        ConcurrentDictionary<string, SeriesInfo> SeriesInfos { get; }
        ConcurrentDictionary<string, Season> Seasons { get; }
        ConcurrentDictionary<string, MxfPerson> People { get; }

        Task<MxfProgram> FindOrCreateProgram(string programId, string md5);
        MxfProgram? FindProgram(string programId);
        void RemoveProgram(string programId);
        Season FindOrCreateSeason(string seriesId, int seasonNumber, string ProgramId);
        SeriesInfo FindOrCreateSeriesInfo(string seriesId, string ProgramId);
        SeriesInfo? FindSeriesInfo(string seriesId);
        MxfPerson FindOrCreatePerson(string name);
    }
}