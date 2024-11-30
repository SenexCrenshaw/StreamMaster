namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IDescriptions : IEPGCached, IDisposable
    {
        Task<bool> BuildGenericSeriesInfoDescriptionsAsync(CancellationToken cancellationToken);
    }
}