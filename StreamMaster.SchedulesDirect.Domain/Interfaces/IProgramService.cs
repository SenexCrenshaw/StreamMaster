namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IProgramService : IDisposable, IEPGCached
    {
        Task<bool> BuildProgramEntriesAsync(CancellationToken cancellationToken);
    }
}