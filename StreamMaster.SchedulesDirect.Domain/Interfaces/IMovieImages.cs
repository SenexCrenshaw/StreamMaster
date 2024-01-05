namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IMovieImages : IEPGCached
    {
        Task<bool> GetAllMoviePosters();
    }
}