namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IScheduleService : IEPGCached
    {
        Task<bool> BuildScheduleAndProgramEntriesAsync(CancellationToken cancellationToken);
    }
}