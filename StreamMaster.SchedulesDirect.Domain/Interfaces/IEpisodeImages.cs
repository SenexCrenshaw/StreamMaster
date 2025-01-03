namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IEpisodeImages : IEPGCached, IDisposable
    {
        Task<bool> ProcessArtAsync(CancellationToken cancellationToken);
    }
}
