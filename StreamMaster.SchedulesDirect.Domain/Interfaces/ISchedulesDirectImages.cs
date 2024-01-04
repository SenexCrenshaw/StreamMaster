using StreamMaster.SchedulesDirect.Domain.Models;

using System.Collections.Specialized;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface ISchedulesDirectImages : IEPGCached
    {
        NameValueCollection sportsSeries { get; set; }
        List<MxfProgram> sportEvents { get; set; }
        Task<List<ProgramMetadata>?> GetArtworkAsync(string[] request);
        Task<bool> GetAllSeriesImages();
        Task<bool> GetAllSportsImages();
        Task<bool> GetAllSeasonImages();
        Task<bool> GetAllMoviePosters();
    }
}