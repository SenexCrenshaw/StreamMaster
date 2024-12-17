namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface ILineupService : IEPGCached, IDisposable
    {
        Task<bool> BuildLineupServicesAsync(CancellationToken cancellationToken = default);

        Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken);
    }
}