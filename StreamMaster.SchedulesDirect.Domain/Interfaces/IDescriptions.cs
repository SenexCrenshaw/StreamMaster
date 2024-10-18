namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IDescriptions : IEPGCached, IDisposable
    {
        Task<bool> BuildAllGenericSeriesInfoDescriptions(CancellationToken cancellationToken);
    }
}