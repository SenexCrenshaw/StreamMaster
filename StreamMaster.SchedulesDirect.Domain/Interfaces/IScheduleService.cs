namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IScheduleService : IEPGCached
    {
        Task<bool> GetAllScheduleEntryMd5S(CancellationToken cancellationToken);
    }
}