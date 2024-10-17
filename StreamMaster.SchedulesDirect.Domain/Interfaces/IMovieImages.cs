namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IMovieImages : IEPGCached, IDisposable
    {
        Task<bool> GetAllMoviePosters();
    }
}