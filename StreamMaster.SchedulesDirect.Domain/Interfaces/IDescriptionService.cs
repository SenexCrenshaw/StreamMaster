namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IDescriptionService : IEPGCached, IDisposable
    {
        Task<bool> BuildGenericSeriesInfoDescriptionsAsync(CancellationToken cancellationToken);
    }
}