namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface ISchedules : IEPGCached
    {
        Task<bool> GetAllScheduleEntryMd5S(CancellationToken cancellationToken);
    }
}