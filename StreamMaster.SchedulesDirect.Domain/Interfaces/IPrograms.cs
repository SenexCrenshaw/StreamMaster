namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IPrograms : IEPGCached
    {
        Task<bool> BuildAllProgramEntries(CancellationToken cancellationToken);
    }
}