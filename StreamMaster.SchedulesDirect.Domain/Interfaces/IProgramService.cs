namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IProgramService : IEPGCached, IDisposable
    {
        Task<bool> BuildAllProgramEntries(CancellationToken cancellationToken);
    }
}