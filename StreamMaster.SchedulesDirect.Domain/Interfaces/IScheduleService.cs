namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IScheduleService : IEPGCached
    {
        Task<bool> BuildScheduleEntriesAsync(CancellationToken cancellationToken);
    }
}